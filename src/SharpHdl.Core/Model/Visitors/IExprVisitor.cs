namespace SharpHdl.Core.Model.Visitors;

public interface IExprVisitor<T>
{
	T VisitSignal(Signal signal);
	T VisitOp(OpExpr opExpr);
	T VisitSlice(SliceExpr sliceExpr);
	T VisitConcat(ConcatExpr concatExpr);
	T VisitSignExtend(SignExtendExpr signExtendExpr);
	T VisitZeroExtend(ZeroExtendExpr zeroExtendExpr);
	T VisitConst(ConstExpr constExpr);
}