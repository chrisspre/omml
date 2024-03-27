﻿namespace Csdl.Graph;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using CommandLine;

internal class Program
{
    private static void Main(string[] args)
    {
        Parser.Default
            .ParseArguments<ProgramOptions>(args)
            .WithParsed<ProgramOptions>(o =>
            {
                var outputFile = o.OutputFile?.FullName ?? Path.ChangeExtension(o.InputFile.FullName, ".md");
                Run(o.InputFile.FullName, outputFile);
            });
    }

    private static void Run(string inputFile, string outputFile)
    {
        var schema = LabeledPropertyGraphSchema.Default;
        var outputDir = Path.GetDirectoryName(outputFile)!;
        File.WriteAllText(Path.Combine(outputDir, "meta-model-schema.md"), schema.ToString());

        schema.JsonSerialize(Path.Combine(outputDir, "meta-model-schema.json"));

        var graph = Graph.LoadGraph(schema, inputFile);

        graph.WriteTo(outputFile);
    }
}



public class ProgramOptions
{
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; init; }

    [Option('i', "input", Required = true)]
    required public FileInfo InputFile { get; init; }

    [Option('o', "output", Required = false)]
    required public FileInfo? OutputFile { get; init; }
}