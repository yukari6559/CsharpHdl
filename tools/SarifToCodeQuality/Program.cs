using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

// Converts SARIF 2.1 files from the compiler (ErrorLog) into the GitLab Code Quality report
// format (a Code Climate issue array). Paths become relative to the repository root.
// Usage: SarifToCodeQuality <repo-root> <out.json> <file.sarif>...
if (args.Length < 2)
{
	Console.Error.WriteLine("usage: SarifToCodeQuality <repo-root> <out.json> <file.sarif>...");
	return 2;
}

string root = Path.GetFullPath(args[0]);
string output = args[1];
JsonArray issues = [];
HashSet<string> fingerprints = [];

foreach (string sarifPath in args.Skip(2))
{
	JsonNode? sarif = JsonNode.Parse(File.ReadAllText(sarifPath));
	foreach (JsonNode? run in sarif?["runs"]?.AsArray() ?? [])
	{
		foreach (JsonNode? result in run?["results"]?.AsArray() ?? [])
		{
			// SARIF keeps results suppressed in source ([SuppressMessage], #pragma) with a suppressions entry.
			if (result is null
				|| (string?)result["kind"] is "pass" or "notApplicable"
				|| result["suppressions"]?.AsArray().Count > 0)
			{
				continue;
			}

			string ruleId = (string?)result["ruleId"] ?? "unknown";
			string message = (string?)result["message"]?["text"] ?? ruleId;
			JsonNode? location = result["locations"]?[0]?["physicalLocation"];
			string path = RelativePath(root, (string?)location?["artifactLocation"]?["uri"]);
			int line = (int?)location?["region"]?["startLine"] ?? 1;

			// The line is left out of the fingerprint so findings survive unrelated edits above them.
			string fingerprint = Fingerprint($"{ruleId}|{path}|{message}");
			if (!fingerprints.Add(fingerprint))
			{
				continue;
			}

			issues.Add(new JsonObject
			{
				["description"] = $"{ruleId}: {message}",
				["check_name"] = ruleId,
				["fingerprint"] = fingerprint,
				["severity"] = Severity((string?)result["level"]),
				["location"] = new JsonObject
				{
					["path"] = path,
					["lines"] = new JsonObject { ["begin"] = line },
				},
			});
		}
	}
}

JsonSerializerOptions options = new()
{
	WriteIndented = true,
	Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
};
File.WriteAllText(output, issues.ToJsonString(options));
Console.WriteLine(string.Create(CultureInfo.InvariantCulture, $"code quality: {issues.Count} finding(s) -> {output}"));
return 0;

static string RelativePath(string root, string? uri)
{
	if (string.IsNullOrEmpty(uri))
	{
		return ".";
	}

	string path = Uri.TryCreate(uri, UriKind.Absolute, out Uri? parsed) && parsed.IsFile ? parsed.LocalPath : uri;
	return Path.GetRelativePath(root, path).Replace('\\', '/');
}

static string Fingerprint(string value)
{
	return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

static string Severity(string? level)
{
	return level switch
	{
		"error" => "critical",
		"warning" => "major",
		"note" => "minor",
		_ => "info",
	};
}
