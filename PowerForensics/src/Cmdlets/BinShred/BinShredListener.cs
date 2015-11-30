//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.5.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from .\BinShred.g4 by ANTLR 4.5.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="BinShredParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.5.1")]
[System.CLSCompliant(false)]
public interface IBinShredListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.template"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTemplate([NotNull] BinShredParser.TemplateContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.template"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTemplate([NotNull] BinShredParser.TemplateContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.templateEntry"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTemplateEntry([NotNull] BinShredParser.TemplateEntryContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.templateEntry"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTemplateEntry([NotNull] BinShredParser.TemplateEntryContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.parseRule"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterParseRule([NotNull] BinShredParser.ParseRuleContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.parseRule"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitParseRule([NotNull] BinShredParser.ParseRuleContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.ruleBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRuleBody([NotNull] BinShredParser.RuleBodyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.ruleBody"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRuleBody([NotNull] BinShredParser.RuleBodyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.propertyName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPropertyName([NotNull] BinShredParser.PropertyNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.propertyName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPropertyName([NotNull] BinShredParser.PropertyNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.lookupTableName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLookupTableName([NotNull] BinShredParser.LookupTableNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.lookupTableName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLookupTableName([NotNull] BinShredParser.LookupTableNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.ruleOptions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRuleOptions([NotNull] BinShredParser.RuleOptionsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.ruleOptions"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRuleOptions([NotNull] BinShredParser.RuleOptionsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.byteOption"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterByteOption([NotNull] BinShredParser.ByteOptionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.byteOption"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitByteOption([NotNull] BinShredParser.ByteOptionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.sizeReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSizeReference([NotNull] BinShredParser.SizeReferenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.sizeReference"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSizeReference([NotNull] BinShredParser.SizeReferenceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.byteFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterByteFormat([NotNull] BinShredParser.ByteFormatContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.byteFormat"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitByteFormat([NotNull] BinShredParser.ByteFormatContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.lookupTable"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLookupTable([NotNull] BinShredParser.LookupTableContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.lookupTable"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLookupTable([NotNull] BinShredParser.LookupTableContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.lookupTableEntry"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLookupTableEntry([NotNull] BinShredParser.LookupTableEntryContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.lookupTableEntry"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLookupTableEntry([NotNull] BinShredParser.LookupTableEntryContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.lookupTableEntryKey"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLookupTableEntryKey([NotNull] BinShredParser.LookupTableEntryKeyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.lookupTableEntryKey"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLookupTableEntryKey([NotNull] BinShredParser.LookupTableEntryKeyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="BinShredParser.label"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLabel([NotNull] BinShredParser.LabelContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="BinShredParser.label"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLabel([NotNull] BinShredParser.LabelContext context);
}