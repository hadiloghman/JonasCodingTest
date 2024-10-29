using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model.Models
{
    public class ErrorHandled : ErrorGeneral
    {

        public static ErrorHandled NotFound(
            string uniqueValue,
            string objectName) => new ErrorHandled
            {
                Title = "Error: Code does not exist",
                Description = $"There is no {objectName} with key = {uniqueValue}"
            };

        public static ErrorHandled DuplicateCode(
            string uniqueValue,
            string objectName) => new ErrorHandled
            {
                Title = "Error: Duplicate code",
                Description = $"There is already {objectName} with key = {uniqueValue}"
            };

        public static ErrorHandled ArgumentNotSpecified(params object[] argumentNames) => new ErrorHandled
        {
            Title = "Error: Null argument",
            Description = $"Argument(s) {string.Join(", ", argumentNames)} is not specified",
        };

        public static ErrorHandled RelationExists(
            string parentName,
            string parentId,
            string childName) => new ErrorHandled
            {
                Title = "Error: Reference exists",
                Description = $"{parentName} = {parentId} is used in {childName}",
            };

    }
}
