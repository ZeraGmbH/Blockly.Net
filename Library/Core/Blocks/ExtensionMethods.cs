

using BlocklyNet.Core.Blocks.Controls;
using BlocklyNet.Core.Blocks.Lists;
using BlocklyNet.Core.Blocks.Logic;
using BlocklyNet.Core.Blocks.Math;
using BlocklyNet.Core.Blocks.Procedures;
using BlocklyNet.Core.Blocks.Text;
using BlocklyNet.Core.Blocks.Variables;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks;

/// <summary>
/// 
/// </summary>
public static class Extensions
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="mutations"></param>
  /// <param name="name"></param>
  /// <param name="domain"></param>
  /// <returns></returns>
  public static string GetValue(this IList<Mutation> mutations, string name, string domain = "mutation")
  {
    var mut = mutations.FirstOrDefault(x => x.Domain == domain && x.Name == name);
    if (null == mut)
      return null!;
    return mut.Value;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="context"></param>
  /// <returns></returns>
  public static Context GetRootContext(this Context context)
  {
    var parentContext = context?.Parent;

    while (parentContext != null)
    {
      if (parentContext.Parent == null)
        return parentContext;

      parentContext = parentContext.Parent;
    }

    return context!;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="parser"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public static T AddStandardBlocks<T>(this T parser) where T : Parser<T>
  {
    parser.AddBlock<ControlsRepeatExt>("controls_repeat_ext");
    parser.AddBlock<ControlsIf>("controls_if");
    parser.AddBlock<ControlsWhileUntil>("controls_whileUntil");
    parser.AddBlock<ControlsFlowStatement>("controls_flow_statements");
    parser.AddBlock<ControlsForEach>("controls_forEach");
    parser.AddBlock<ControlsFor>("controls_for");

    parser.AddBlock<LogicCompare>("logic_compare");
    parser.AddBlock<LogicBoolean>("logic_boolean");
    parser.AddBlock<LogicNegate>("logic_negate");
    parser.AddBlock<LogicOperation>("logic_operation");
    parser.AddBlock<LogicNull>("logic_null");
    parser.AddBlock<LogicTernary>("logic_ternary");

    parser.AddBlock<MathArithmetic>("math_arithmetic");
    parser.AddBlock<MathNumber>("math_number");
    parser.AddBlock<MathSingle>("math_single");
    parser.AddBlock<MathSingle>("math_trig");
    parser.AddBlock<MathRound>("math_round");
    parser.AddBlock<MathConstant>("math_constant");
    parser.AddBlock<MathNumberProperty>("math_number_property");
    parser.AddBlock<MathOnList>("math_on_list");
    parser.AddBlock<MathConstrain>("math_constrain");
    parser.AddBlock<MathModulo>("math_modulo");
    parser.AddBlock<MathRandomFloat>("math_random_float");
    parser.AddBlock<MathRandomInt>("math_random_int");
    parser.AddBlock<MathChange>("math_change");

    parser.AddBlock<TextAppend>("text_append");
    parser.AddBlock<TextBlock>("text");
    parser.AddBlock<TextCaseChange>("text_changeCase");
    parser.AddBlock<TextIndexOf>("text_indexOf");
    parser.AddBlock<TextIsEmpty>("text_isEmpty");
    parser.AddBlock<TextJoin>("text_join");
    parser.AddBlock<TextLength>("text_length");
    parser.AddBlock<TextSubstring>("text_getSubstring");
    parser.AddBlock<TextTrim>("text_trim");
    parser.AddBlock<TextReplace>("text_replace");

    parser.AddBlock<VariablesGet>("variables_get");
    parser.AddBlock<VariablesSet>("variables_set");

    parser.AddBlock<ColourPicker>("colour_picker");
    parser.AddBlock<ColourRandom>("colour_random");
    parser.AddBlock<ColourRgb>("colour_rgb");
    parser.AddBlock<ColourBlend>("colour_blend");

    parser.AddBlock<ProceduresDef>("procedures_defnoreturn");
    parser.AddBlock<ProceduresDef>("procedures_defreturn");
    parser.AddBlock<ProceduresCallNoReturn>("procedures_callnoreturn");
    parser.AddBlock<ProceduresCallReturn>("procedures_callreturn");
    parser.AddBlock<ProceduresIfReturn>("procedures_ifreturn");

    parser.AddBlock<ListsSplit>("lists_split");
    parser.AddBlock<ListsCreateWith>("lists_create_with");
    parser.AddBlock<ListsLength>("lists_length");
    parser.AddBlock<ListsRepeat>("lists_repeat");
    parser.AddBlock<ListsIsEmpty>("lists_isEmpty");
    parser.AddBlock<ListsGetIndex>("lists_getIndex");
    parser.AddBlock<ListsSetIndex>("lists_setIndex");
    parser.AddBlock<ListsIndexOf>("lists_indexOf");

    return parser;
  }
}