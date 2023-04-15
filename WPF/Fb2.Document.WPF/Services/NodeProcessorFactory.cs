using System;
using System.Collections.Generic;
using System.Threading;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.WPF.NodeProcessors;
using Fb2.Document.WPF.NodeProcessors.Base;
using Fb2Image = Fb2.Document.Models.Image;
using Fb2Paragraph = Fb2.Document.Models.Paragraph;

namespace Fb2.Document.WPF.Services;

public class NodeProcessorFactory
{
    private static readonly Lazy<NodeProcessorFactory> instance = new Lazy<NodeProcessorFactory>(() => new NodeProcessorFactory(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static NodeProcessorFactory Instance => instance.Value;

    private readonly Dictionary<Type, NodeProcessorBase> nodeMap = new Dictionary<Type, NodeProcessorBase>
        {
            { typeof(Table), new TableProcessor() },
            { typeof(Fb2Image), new ImageProcessor() },
            { typeof(Emphasis), new ItalicProcessor() },
            { typeof(Strong), new BoldProcessor() },
            { typeof(Superscript), new SupescriptProcessor() },
            { typeof(Subscript), new SubscriptProcessor() },
            { typeof(TextLink), new HyperlinkProcessor() },
            { typeof(Author), new AuthorProcessor() },
            { typeof(EmptyLine), new EmptyLineProcessor() },
            { typeof(SequenceInfo), new SequenceProcessor() },
            { typeof(CustomInfo), new CustomInfoProcessor() }
        };

    private readonly HashSet<Type> paragraphElements = new HashSet<Type>
        {
            typeof(Fb2Paragraph),
            typeof(StanzaVerse),
            typeof(Poem),
            typeof(Epigraph),
            typeof(Quote),
            typeof(TextAuthor),
            typeof(SubTitle),
            typeof(Title),
            typeof(BookTitle),
            typeof(BookName),
            typeof(Date),
            typeof(PublishInfo),
            typeof(Publisher),
            typeof(City),
            typeof(Year),
            typeof(ISBNInfo)
        };

    private readonly HashSet<Type> spanElements = new HashSet<Type>
        {
            typeof(Strikethrough),
            typeof(Code),
            typeof(TableCell),
            typeof(TableHeader),
            typeof(FirstName),
            typeof(MiddleName),
            typeof(LastName),
            typeof(Nickname),
            typeof(Email),
            typeof(HomePage)
        };

    public ParagraphProcessor ParagraphProcessor { get; }

    public SpanProcessor SpanProcessor { get; }

    public DefaultNodeProcessor DefaultProcessor { get; }

    private NodeProcessorFactory()
    {
        ParagraphProcessor = new ParagraphProcessor();
        SpanProcessor = new SpanProcessor();
        DefaultProcessor = new DefaultNodeProcessor();
    }

    public NodeProcessorBase GetNodeProcessor(Fb2Node node)
    {
        var currentNodeType = node.GetType();

        if (paragraphElements.Contains(currentNodeType))
            return ParagraphProcessor;

        if (spanElements.Contains(currentNodeType))
            return SpanProcessor;

        if (nodeMap.ContainsKey(currentNodeType))
            return nodeMap[currentNodeType];

        return DefaultProcessor;
    }
}

