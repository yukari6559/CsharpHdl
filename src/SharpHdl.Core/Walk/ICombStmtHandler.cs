using SharpHdl.Core.Model;

namespace SharpHdl.Core.Walk;

public interface ICombStmtHandler
{
	void OnAssign(AssignStmt assignStmt);
	void OnSwitch(SwitchStmt switchStmt);
	void OnIf(IfStmt ifStmt);
	void OnInstance(InstanceStmt instanceStmt);
	void OnMem2R1WStmt(Mem2R1WStmt mem2R1WStmt);
}