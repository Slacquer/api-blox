using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using APIBlox.AspNetCore.Contracts;
using APIBlox.NetCore.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

// ReSharper disable once CheckNamespace
namespace APIBlox.AspNetCore
{
    internal class DynamicControllerSubRouteConvention : IApplicationModelConvention
    {
        private readonly IInternalDynamicControllerConfigurationsService _controllerConfigService;

        public DynamicControllerSubRouteConvention(IInternalDynamicControllerConfigurationsService controllerConfigService)
        {
            _controllerConfigService = controllerConfigService;
        }

        /// <inheritdoc />
        public void Apply(ApplicationModel application)
        {
            PopulateParameters(application);

            // list of subs
            var subControllers = _controllerConfigService.ControllerConfigurations
                .Flatten(c => c.SubResourceControllers, c => !(c.ParentController is null))
                .ToList();

            if (!subControllers.Any())
                return;

            // For each sub, using its parent, we need to add a parameter to ALL of its actions.
            foreach (var subController in subControllers)
                ProcessController(application, subController);
        }

        private static string GetParentNameId(DynamicControllerConfiguration parentConfig)
        {
            var first = Regex.Matches(parentConfig.RequestResourceType.Name, "[A-Z][^A-Z]*")[0];

            return $"{first}Id".ToCamelCase();
        }

        private Dictionary<string, ParameterModel> BuildListOfParentIds(DynamicControllerConfiguration subController)
        {
            var ret = new Dictionary<string, ParameterModel>();
            var current = subController;

            while (!(current is null))
            {
                var match = _controllerConfigService.Parameters.FirstOrDefault(pm =>
                    pm.ParameterType == current.ParentIdType
                );

                if (match is null)
                    return ret;

                ret.Add(GetParentNameId(current.ParentController), match);
                current = current.ParentController;
            }

            return ret;
        }

        private void PopulateParameters(ApplicationModel application)
        {
            // TODO: Figure out how to create parameter WITHOUT cloning another.
            var dummy = application.Controllers.First(c =>
                c.ControllerName == "HowDoICreateAParameterModelWithoutDoingThisCrap"
            );
            _controllerConfigService.Parameters = dummy.Actions.First().Parameters.ToList();
            //_controllerConfigService.RequiredParameters = dummy.Actions.Last().Parameters.ToList();
            application.Controllers.Remove(dummy);
        }

        private void ProcessController(ApplicationModel application, DynamicControllerConfiguration subController)
        {
            // Get a list of parent ids traversing UP
            var parentIds = BuildListOfParentIds(subController);

            if (!parentIds.Any())
                return;

            var actions = application.Controllers
                .Where(cm => cm.ControllerType == subController.ControllerType)
                .SelectMany(cm => cm.Actions)
                .ToList();

            // for each action in the controller, insert all the parentIds.
            foreach (var action in actions)
            {
                foreach (var kvp in parentIds)
                    action.Parameters.Insert(0, new ParameterModel(kvp.Value) {ParameterName = kvp.Key});
            }
        }
    }
}
