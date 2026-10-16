using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Model;
using SharpHdl.Core.Model.Visitors;

namespace SharpHdl.Core.Validate;

public class CheckUndeclaredLocalsVisitor(Module module) : IStmtVisitor
{
	private readonly HashSet<Signal> declared = [.. module.GetLocals()];
	public void VisitAssign(AssignStmt assignStmt)
	{
		CollectSignalsExprVisitor collectSignalsExprVisitor = new();
		CheckUsed(assignStmt.Signal);
		foreach (Signal signal in assignStmt.Expr.Accept(collectSignalsExprVisitor))
		{
			CheckUsed(signal);
		}
	}

	public void VisitIf(IfStmt ifStmt)
	{
		CollectSignalsExprVisitor collectSignalsExprVisitor = new();
		foreach (Signal signal in ifStmt.Cond.Accept(collectSignalsExprVisitor))
		{
			CheckUsed(signal);
		}
		foreach (Stmt thenStmt in ifStmt.Then)
		{
			thenStmt.Accept(this);
		}
		if (ifStmt.Else is not null)
		{
			foreach (Stmt elseStmt in ifStmt.Else)
			{
				elseStmt.Accept(this);
			}
		}
	}

	public void VisitInstance(InstanceStmt instanceStmt)
	{
		foreach (PortConnection connection in instanceStmt.PortConnections)
		{
			CheckUsed(connection.ParentSignal);
		}
		CheckUndeclaredLocals.Check(instanceStmt.ChildModule);
	}

	public void VisitMem(MemStmt memStmt)
	{
	}

	public void VisitMem2R1W(Mem2R1WStmt mem2R1WStmt)
	{
	}

	public void VisitMemWstrb(MemWstrbStmt memWstrbStmt)
	{
	}

	public void VisitSeqAssign(SeqAssignStmt seqAssignStmt)
	{
		CollectSignalsExprVisitor collectSignalsExprVisitor = new();
		CheckUsed(seqAssignStmt.Signal);
		foreach (Signal signal in seqAssignStmt.Expr.Accept(collectSignalsExprVisitor))
		{
			CheckUsed(signal);
		}
	}

	public void VisitSeqBlock(SeqBlockStmt seqBlockStmt)
	{
		foreach (Stmt stmt in seqBlockStmt.Body)
		{
			stmt.Accept(this);
		}
	}

	public void VisitSwitch(SwitchStmt switchStmt)
	{
		CollectSignalsExprVisitor collectSignalsExprVisitor = new();
		foreach (Signal signal in switchStmt.Expr.Accept(collectSignalsExprVisitor))
		{
			CheckUsed(signal);
		}
		foreach (Case item in switchStmt.Cases)
		{
			foreach (Stmt stmt in item.Stmts)
			{
				stmt.Accept(this);
			}
		}
	}

	private void CheckUsed(Signal signal)
	{
		if (signal.Direction is SignalDirection.Internal)
		{
			if (!declared.Contains(signal))
			{
				throw new SignalDeclarationException($"Signal '{signal.Name}' is used but not declared with SetLocal/SetLocals.");
			}
		}
	}
}