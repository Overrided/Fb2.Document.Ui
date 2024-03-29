﻿using System.Windows;

namespace Fb2.Document.WPF.Entities;

public class PoemConfig : SectionConfig
{
    public TextAlignment VerseHorizontalAlignment { get; }

    public TextAlignment TextAuthorHorizontalAlignment { get; }

    public TextAlignment DateHorizontalAlignment { get; }

    public PoemConfig(
        TextAlignment titleHorizontalAlignment = TextAlignment.Center,
        TextAlignment subtitleHorizontalAlignment = TextAlignment.Center,
        TextAlignment epigraphHorizontalAlignment = TextAlignment.Right,
        TextAlignment verseHorizontalAlignment = TextAlignment.Center,
        TextAlignment textAuthorHorizontalAlignment = TextAlignment.Right,
        TextAlignment dateHorizontalAlignment = TextAlignment.Right)
        : base(titleHorizontalAlignment, subtitleHorizontalAlignment, epigraphHorizontalAlignment)
    {
        VerseHorizontalAlignment = verseHorizontalAlignment;
        TextAuthorHorizontalAlignment = textAuthorHorizontalAlignment;
        DateHorizontalAlignment = dateHorizontalAlignment;
    }
}

public class SectionConfig : ConfigBase
{
    public TextAlignment SubtitleHorizontalAlignment { get; }

    public SectionConfig(
        TextAlignment titleHorizontalAlignment = TextAlignment.Center,
        TextAlignment subtitleHorizontalAlignment = TextAlignment.Center,
        TextAlignment epigraphHorizontalAlignment = TextAlignment.Right)
        : base(titleHorizontalAlignment, epigraphHorizontalAlignment)
    {
        SubtitleHorizontalAlignment = subtitleHorizontalAlignment;
    }
}

public class ConfigBase
{
    public TextAlignment TitleHorizontalAlignment { get; }
    public TextAlignment EpigraphHorizontalAlignment { get; }

    public ConfigBase(
        TextAlignment titleHorizontalAlignment = TextAlignment.Center,
        TextAlignment epigraphHorizontalAlignment = TextAlignment.Right)
    {
        TitleHorizontalAlignment = titleHorizontalAlignment;
        EpigraphHorizontalAlignment = epigraphHorizontalAlignment;
    }
}

public class QuoteConfig
{
    public TextAlignment SubtitleHorizontalAlignment { get; }
    public TextAlignment TextAuthorHorizontalAlignment { get; }

    public QuoteConfig(
        TextAlignment subtitleHorizontalAlignment = TextAlignment.Center,
        TextAlignment textAuthorHorizontalAlignment = TextAlignment.Right)
    {
        SubtitleHorizontalAlignment = subtitleHorizontalAlignment;
        TextAuthorHorizontalAlignment = textAuthorHorizontalAlignment;
    }
}

public class AnnotationConfig
{
    public TextAlignment SubTitleHorizontalAlignment { get; }

    public AnnotationConfig(TextAlignment subTitleHorizontalAlignment = TextAlignment.Center)
    {
        SubTitleHorizontalAlignment = subTitleHorizontalAlignment;
    }
}

public class ImageConfig
{
    public string NonInlineImageTag { get; set; }

    // for future use, not utilized yet
    public string NonInlineImageContainerTag { get; set; }

    public ImageConfig(string nonInlineImageTag = "NotInlineFb2ImageTag", string nonInlineImageContainerTag = null)
    {
        NonInlineImageTag = nonInlineImageTag;
        NonInlineImageContainerTag = nonInlineImageContainerTag;
    }
}

// TODO : add bool UseStyles / UseDefaultStyles
public class Fb2MappingConfig
{
    public int BaseFontSize { get; set; } = 18;

    public double ParagraphIndent { get; set; } = 20;

    public bool HighlightUnsafe { get; set; } = false;

    public bool UseStyles { get; set; } = true;

    public PoemConfig Poem { get; set; } = new PoemConfig();

    public ConfigBase Body { get; set; } = new ConfigBase();

    public SectionConfig Section { get; set; } = new SectionConfig();

    public QuoteConfig Quote { get; set; } = new QuoteConfig();

    public AnnotationConfig Annotation { get; set; } = new AnnotationConfig();

    public ImageConfig Image { get; set; } = new ImageConfig();

    public Fb2MappingConfig()
    {
    }

    public Fb2MappingConfig(
        int baseFontSize = 18,
        double paragraphIndent = 20,
        bool highlightUnsafe = false,
        bool useStyles = true,
        PoemConfig? poemConfig = null,
        ConfigBase? bodyConfig = null,
        SectionConfig? bodySectionConfig = null,
        QuoteConfig? citeConfig = null,
        AnnotationConfig? annotationConfig = null,
        ImageConfig? imageConfig = null)
    {
        BaseFontSize = baseFontSize;
        ParagraphIndent = paragraphIndent;
        HighlightUnsafe = highlightUnsafe;
        UseStyles = useStyles;

        Poem = poemConfig ?? new PoemConfig();
        Body = bodyConfig ?? new ConfigBase();
        Section = bodySectionConfig ?? new SectionConfig();
        Quote = citeConfig ?? new QuoteConfig();
        Annotation = annotationConfig ?? new AnnotationConfig();
        Image = imageConfig ?? new ImageConfig();
    }
}

public class Fb2DocumentMappingConfig : Fb2MappingConfig
{
    public bool MapWholeDocument { get; } = false;

    public Fb2DocumentMappingConfig()
    {
    }

    public Fb2DocumentMappingConfig(
        bool mapWholeDocument = false,
        int baseFontSize = 18,
        double paragraphIndent = 20,
        bool highlightUnsafe = false,
        bool useStyles = true,
        PoemConfig? poemConfig = null,
        ConfigBase? bodyConfig = null,
        SectionConfig? bodySectionConfig = null,
        QuoteConfig? citeConfig = null,
        AnnotationConfig? annotationConfig = null,
        ImageConfig? imageConfig = null) : base(
            baseFontSize,
            paragraphIndent,
            highlightUnsafe,
            useStyles,
            poemConfig,
            bodyConfig,
            bodySectionConfig,
            citeConfig,
            annotationConfig,
            imageConfig)
    {
        MapWholeDocument = mapWholeDocument;
    }
}

