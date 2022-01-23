using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using CSharpFunctionalExtensions;

namespace Application.Utils
{
    internal static class Utilities
    {
        public static void SetEntityId<T>(this T entity, Guid idToSet) where T : Entity<Guid>
        {
            var entitySpecFound = entity.GetType();
            while (true)
            {
                if (entitySpecFound.Namespace == "CSharpFunctionalExtensions"
                 && entitySpecFound.Name.StartsWith("Entity")
                 && entitySpecFound.BaseType.Name == "Object")
                    break;

                entitySpecFound = entitySpecFound.BaseType;
            }

            entitySpecFound
                .GetRuntimeFields()
                .Where(a => Regex.IsMatch(a.Name, $"\\A<{nameof(Entity.Id)}>k__BackingField\\Z"))
                .FirstOrDefault()
                .SetValue(entity, idToSet);
        }
    }
}
