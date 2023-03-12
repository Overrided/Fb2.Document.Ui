using System;
using System.Collections.Generic;
using System.Threading;
using Fb2.Document.Html.NodeProcessors;
using Fb2.Document.Html.NodeProcessors.Base;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;

namespace Fb2.Document.Html.Services;

public class NodeProcessorFactory
{
    private static readonly Lazy<NodeProcessorFactory> instance = new Lazy<NodeProcessorFactory>(() => new NodeProcessorFactory(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static NodeProcessorFactory Instance => instance.Value;

    private readonly Dictionary<Type, Fb2HtmlNodeProcessorBase> nodeMap = new Dictionary<Type, Fb2HtmlNodeProcessorBase>
    {
        //{ typeof(Table), new TableProcessor() },
        //{ typeof(Fb2Image), new ImageProcessor() },
        //{ typeof(Emphasis), new ItalicProcessor() },
        //{ typeof(Strong), new BoldProcessor() },
        //{ typeof(Superscript), new SupescriptProcessor() },
        //{ typeof(Subscript), new SubscriptProcessor() },
        //{ typeof(TextLink), new HyperlinkProcessor() },
        //{ typeof(Author), new AuthorProcessor() },
        { typeof(BookTitle), new TitleProcessor() }, // TODO : BookTitleProcessor
        { typeof(EmptyLine), new EmptyLineProcessor() },
        { typeof(Title), new TitleProcessor() },
        { typeof(Emphasis), new EmphasisProcessor() },
        { typeof(Image), new ImageProcessor() },
        { typeof(SubTitle), new SubTitleProcessor() },
        //{ typeof(SequenceInfo), new SequenceProcessor() },
        //{ typeof(CustomInfo), new CustomInfoProcessor() }
    };

    private readonly HashSet<Type> paragraphElements = new HashSet<Type>
    {
        //    typeof(Fb2Paragraph),
        //    typeof(StanzaVerse),
        //    typeof(Poem),
        //    typeof(Epigraph),
        //    typeof(Quote),
        typeof(SequenceInfo),
        typeof(Author),
        typeof(TextAuthor),
        typeof(StanzaVerse)
        //    typeof(SubTitle),
        //    typeof(Title),
        //    typeof(BookTitle),
        //    typeof(BookName),
        //    typeof(Date),
        //    typeof(PublishInfo),
        //    typeof(Publisher),
        //    typeof(City),
        //    typeof(Year),
        //    typeof(ISBNInfo)
    };

    private readonly HashSet<Type> divElements = new HashSet<Type>
    {
        typeof(BodySection),
        typeof(BookBody),
        typeof(Coverpage),
        typeof(Epigraph),
        typeof(Poem),
        typeof(Stanza)
        //typeof(TextAuthor)
        //typeof(Code),
        //typeof(TableCell),
        //typeof(TableHeader),
        //typeof(FirstName),
        //typeof(MiddleName),
        //typeof(LastName),
        //typeof(Nickname),
        //typeof(Email),
        //typeof(HomePage)
    };

    public ParagraphProcessor ParagraphProcessor { get; }

    //public SpanProcessor SpanProcessor { get; }

    public DivFb2HtmlProcessor DivFb2HtmlProcessor { get; }

    public DefaultFb2HtmlNodeProcessor DefaultProcessor { get; }

    private NodeProcessorFactory()
    {
        ParagraphProcessor = new ParagraphProcessor();
        //SpanProcessor = new SpanProcessor();
        DefaultProcessor = new DefaultFb2HtmlNodeProcessor();
        DivFb2HtmlProcessor = new DivFb2HtmlProcessor();
    }

    public Fb2HtmlNodeProcessorBase GetNodeProcessor(Fb2Node node)
    {
        var currentNodeType = node.GetType();

        if (divElements.Contains(currentNodeType))
            return DivFb2HtmlProcessor;

        if (paragraphElements.Contains(currentNodeType))
            return ParagraphProcessor;

        if (nodeMap.ContainsKey(currentNodeType))
            return nodeMap[currentNodeType];

        return DefaultProcessor;
    }
}
