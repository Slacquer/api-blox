using System.Linq;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class DynamicControllerActionOperationIdConvention : IApplicationModelConvention
    {
        private readonly ILogger<DynamicControllerActionOperationIdConvention> _log;

        public DynamicControllerActionOperationIdConvention(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger<DynamicControllerActionOperationIdConvention>();
        }

        public void Apply(ApplicationModel application)
        {
            var actions = application.Controllers
                .Where(cm => cm.ControllerType.IsGenericType).SelectMany(cm => cm.Actions)
                .ToList();

            foreach (var actionModel in actions)
                Apply(actionModel);
        }

        private void Apply(ActionModel action)
        {
            action.ActionName = Increment($"{action.ActionName}-{action.Controller.ControllerName}", action.Controller);
        }

        private string Increment(string name, ControllerModel controller)
        {
            if (controller.Actions is null || !(controller.Actions.Any()))
                return name;

            var count = controller.Actions.Count(a => a.ActionName.EqualsEx(name));

            if (count <= 0)
                return name;

            var newValue = $"{name}-{count + 1}";

            _log.LogWarning(() => $"Duplicate action name (operationId) found, {name}, incrementing for new value of {newValue}.");

            return newValue;

        }
    }
}
