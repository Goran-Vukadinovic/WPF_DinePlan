#region using

using System;
using System.ComponentModel.Composition;
using DinePlan.Domain.Models.Menus;
using DinePlan.Localization.Properties;
using DinePlan.Presentation.Common.ModelBase;
using FluentValidation;

#endregion

namespace DinePlan.Modules.MenuModule
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class CourseViewModel : EntityViewModelBase<Course>
    {
        [ImportingConstructor]
        public CourseViewModel()
        {
        }

        public int? Priority
        {
            get { return Model.Priority; }
            set { Model.Priority = value; }
        }

        public override Type GetViewType()
        {
            return typeof (CourseView);
        }

        public override string GetModelTypeString()
        {
            return Resources.Courses;
        }

        protected override AbstractValidator<Course> GetValidator()
        {
            return new CourseValidator();
        }
    }

    internal class CourseValidator : EntityValidator<Course>
    {
        public CourseValidator()
        {
            RuleFor(x => x.Priority).NotEmpty();
            RuleFor(x => x.Priority).GreaterThan(10);
        }
    }
}