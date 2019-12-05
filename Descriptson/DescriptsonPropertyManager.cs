using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Descriptson
{
    internal static class DescriptsonPropertyManager<TTarget>
    {
        private const char indexerEnd = ']';
        private const char indexerSeperator = ',';
        private const char indexerStart = '[';
        private const string indexPropertyName = "Item";
        private const char property = '.';

        private static readonly Dictionary<string, PropertyInfo> getProperties =
            typeof(TTarget).GetProperties()
            .Where(p => p.GetCustomAttribute<DescriptsonGetAttribute>() != null && p.Name != indexPropertyName)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        private static readonly char[] memberAccess = new[] { property, indexerStart };

        private static readonly Dictionary<string, PropertyInfo> setProperties =
            typeof(TTarget).GetProperties()
            .Where(p => p.GetCustomAttribute<DescriptsonSetAttribute>() != null && p.Name != indexPropertyName)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        private static readonly ParameterExpression targetParam = Expression.Parameter(typeof(TTarget));

        public static Expression<Func<TTarget, object>> ParseAccessPath(string path)
        {
            return Expression.Lambda<Func<TTarget, object>>(parseAccessPath(path), targetParam);
        }

        private static int indexOfMatchingChar(string str, char open, char close, int startIndex)
        {
            var depth = 1;
            var nextIndex = -1;
            var search = new[] { open, close };

            while (depth > 0 && (nextIndex = str.IndexOfAny(search, startIndex)) >= 0)
            {
                if (str[nextIndex] == open)
                    ++depth;
                else if (str[nextIndex] == close)
                    --depth;

                startIndex = nextIndex + 1;
            }

            return startIndex - 1;
        }

        private static Expression parseAccessPath(string path, int currentIndex = 0, Expression currentExpression = null)
        {
            currentExpression = currentExpression ?? targetParam;
            var nextIndex = path.IndexOfAny(memberAccess, currentIndex);

            // if no more accessors found, only the tail remains
            if (nextIndex < 0)
                nextIndex = path.Length - 1;

            // member access before nextIndex
            if (nextIndex > currentIndex)
            {
                var name = path.Substring(currentIndex, nextIndex - currentIndex);
                if (!getProperties.ContainsKey(name))
                    throw new InvalidOperationException(
                        $"Property [{name}] on [{typeof(TTarget).FullName}] doesn't exist or needs the {nameof(DescriptsonGetAttribute)} to be read!");

                currentExpression = Expression.MakeMemberAccess(currentExpression, getProperties[name]);
            }
            // indexer
            else if (path[nextIndex] == indexerStart)
            {
                // find closing bracket of this particular indexer
                var indexerEndIndex = indexOfMatchingChar(path, indexerStart, indexerEnd, nextIndex + 1);

                // split this particular indexer into sub-paths and parse them
                var indexParameterStr = path.Substring(currentIndex, indexerEndIndex - currentIndex);
                var indexParameterPaths = topLevelSplit(indexParameterStr, indexerStart, indexerEnd, indexerSeperator);
                var indexParametersExpressions = indexParameterPaths.Select(p => parseAccessPath(p)).ToArray();

                var indexInfo = typeof(TTarget).GetProperty(indexPropertyName, indexParametersExpressions.Select(p => p.Type).ToArray());
                if (indexInfo == null || indexInfo.GetCustomAttribute<DescriptsonGetAttribute>() != null)
                    throw new InvalidOperationException(
                        $"Indexer on [{typeof(TTarget).FullName}] doesn't exist or needs the {nameof(DescriptsonGetAttribute)} to be read!");

                currentExpression = Expression.MakeIndex(currentExpression, indexInfo, new[] { targetParam });
            }

            currentIndex = nextIndex + 1;

            // reached the end?
            if (currentIndex >= path.Length)
                return currentExpression;

            // otherwise create the property manager for the new "current" value and continue parsing with it
            return (Expression)typeof(DescriptsonPropertyManager<>)
                .MakeGenericType(currentExpression.Type)
                .GetMethod(nameof(parseAccessPath))
                .Invoke(null, new object[] { path, currentIndex, currentExpression });
        }

        private static IEnumerable<string> topLevelSplit(string str, char open, char close, char split)
        {
            var depth = 0;
            var startIndex = 0;
            var nextIndex = -1;
            var search = new[] { open, close, split };

            while ((nextIndex = str.IndexOfAny(search, startIndex)) >= 0)
            {
                if (str[nextIndex] == open)
                    ++depth;
                else if (str[nextIndex] == close)
                    --depth;
                else if (str[nextIndex] == split && depth == 0)
                    yield return str.Substring(startIndex, nextIndex - startIndex);

                startIndex = nextIndex + 1;
            }
        }
    }
}