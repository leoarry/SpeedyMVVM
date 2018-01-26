using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Validation
{
    public enum MergingActionEnum
    {

        /// <summary>
        ///Merge all the rules and rule actions. In case the rule won't exist on the source will be added, in case the source contain the rule then merge rule actions for the rule. 
        /// </summary>
        MergeAll,
        /// <summary>
        /// Replace the source rules collection with the parameter collection.
        /// </summary>
        ReplaceRules,
        /// <summary>
        /// Merge the rules and in case will exist on the source a set of rules for the property, that will be replaced.
        /// </summary>
        ReplacePropertyRules,
        /// <summary>
        /// Merge the rules and in case will exist on the source a set of rules for the property, that will be skipped.
        /// </summary>
        OverridePropertyRules,
        /// <summary>
        /// Merge all the rule actions for the existing property rules.
        /// </summary>
        MergeExistingRuleActions
    }
}
