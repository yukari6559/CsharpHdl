using SharpHdl.Core.Exceptions;
using SharpHdl.Core.Validate;
using SharpHdl.Tests.TestModule;

namespace SharpHdl.Tests;

public class MultiDriveTests
{
	private static void Check(BadMultiDriveModule module)
	{
		CheckMultiDrive.Check([.. module.GetStmts()]);
	}

	[Fact]
	public void AssignAndSwitchSecondCaseThrows()
	{
		BadMultiDriveModule module = new();
		module.DescribeAssignAndSwitchSecondCase();
		_ = Assert.Throws<MultiDriveException>(() => Check(module));
	}

	[Fact]
	public void AssignAndIfThenThrows()
	{
		BadMultiDriveModule module = new();
		module.DescribeAssignAndIfThen();
		_ = Assert.Throws<MultiDriveException>(() => Check(module));
	}

	[Fact]
	public void IfThenBeforeAssignThrows()
	{
		BadMultiDriveModule module = new();
		module.DescribeIfThenBeforeAssign();
		_ = Assert.Throws<MultiDriveException>(() => Check(module));
	}

	[Fact]
	public void DualAssignInIfThenThrows()
	{
		BadMultiDriveModule module = new();
		module.DescribeDualAssignInIfThen();
		_ = Assert.Throws<MultiDriveException>(() => Check(module));
	}

	[Fact]
	public void IfThenElseSameSignalDoesNotThrow()
	{
		BadMultiDriveModule module = new();
		module.DescribeIfThenElseSameSignal();
		Check(module);
	}
}
