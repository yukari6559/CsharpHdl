using System.Text;

/// <summary>
/// Phase 1: StringBuilder で ALU の Verilog を生成する試作。
/// CLI から一時的に呼び出します。
/// </summary>
public static class Alu
{
    public class Wire
    {
        public required string Direction { get; set; }
        public required string Msb { get; set; }
        public required string Lsb { get; set; }
        public required string Name { get; set; }

        public string PortLine(bool trailingComma) =>
            $"  {Direction,-6} wire [{Msb}:{Lsb}] {Name}{(trailingComma ? "," : "")}";
    }

    public static void Describe(string? outputPath = null)
    {
        // リポジトリルート基準の out/generated/alu.v（cwd に依存しないよう解決）
        var path = outputPath ?? ResolveDefaultOutputPath();
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        const string moduleName = "Alu";

        Wire A = new() { Direction = "input", Msb = "31", Lsb = "0", Name = "A" };
        Wire B = new() { Direction = "input", Msb = "31", Lsb = "0", Name = "B" };
        Wire Op = new() { Direction = "input", Msb = "1", Lsb = "0", Name = "Op" };
        Wire Y = new() { Direction = "output", Msb = "31", Lsb = "0", Name = "Y" };

        var sb = new StringBuilder();
        sb.AppendLine($"module {moduleName}(");
        sb.AppendLine(A.PortLine(trailingComma: true));
        sb.AppendLine(B.PortLine(trailingComma: true));
        sb.AppendLine(Op.PortLine(trailingComma: true));
        sb.AppendLine(Y.PortLine(trailingComma: false));
        sb.AppendLine(");");
        sb.AppendLine("  assign Y = (Op == 2'd0) ? (A + B) :");
        sb.AppendLine("             (Op == 2'd1) ? (A - B) :");
        sb.AppendLine("             (Op == 2'd2) ? (A & B) :");
        sb.AppendLine("                           (A | B);");
        sb.AppendLine("endmodule");

        File.WriteAllText(path, sb.ToString());
        Console.WriteLine($"Wrote {path}");
    }

    static string ResolveDefaultOutputPath()
    {
        // src/SharpHdl.Cli/bin/Debug/net10.0 → リポジトリルートへ
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "CsharpHdl.slnx")))
                return Path.Combine(dir.FullName, "out", "generated", "alu.v");
            dir = dir.Parent;
        }

        return Path.GetFullPath(Path.Combine("out", "generated", "alu.v"));
    }
}
