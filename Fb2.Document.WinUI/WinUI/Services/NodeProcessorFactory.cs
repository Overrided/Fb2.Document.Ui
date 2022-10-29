﻿using System;
using System.Collections.Generic;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.UI.WinUi.NodeProcessors;
using Fb2.Document.UI.WinUi.NodeProcessors.Base;
using Fb2Image = Fb2.Document.Models.Image;
using Fb2Paragraph = Fb2.Document.Models.Paragraph;

namespace Fb2.Document.UI.WinUi.Services
{
    public class NodeProcessorFactory
    {
        private ParagraphProcessor paragraphProcessor = null;

        private SpanProcessor spanProcessor = null;

        private DefaultNodeProcessor defaultProcessor = null;

        private ParagraphProcessor ParagraphProcessor
        {
            get
            {
                if (paragraphProcessor == null)
                    paragraphProcessor = new ParagraphProcessor();

                return paragraphProcessor;
            }
        }

        private SpanProcessor SpanProcessor
        {
            get
            {
                if (spanProcessor == null)
                    spanProcessor = new SpanProcessor();

                return spanProcessor;
            }
        }

        public DefaultNodeProcessor DefaultProcessor
        {
            get
            {
                if (defaultProcessor == null)
                    defaultProcessor = new DefaultNodeProcessor();

                return defaultProcessor;
            }
        }

        private readonly Dictionary<Type, NodeProcessorBase> NodeMap = new Dictionary<Type, NodeProcessorBase>
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

        private readonly HashSet<Type> ParagraphElements = new HashSet<Type>
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

        private readonly HashSet<Type> SpanElements = new HashSet<Type>
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

        public NodeProcessorBase GetNodeProcessor(Fb2Node node)
        {
            var currentNodeType = node.GetType();

            if (ParagraphElements.Contains(currentNodeType))
                return ParagraphProcessor;

            if (SpanElements.Contains(currentNodeType))
                return SpanProcessor;

            if (NodeMap.ContainsKey(currentNodeType))
                return NodeMap[currentNodeType];

            return DefaultProcessor;
        }
    }
}