using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fb2.Document.Constants;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.WPF.Common;

namespace Fb2.Document.WPF.Playground.Components;

public class BookGenreViewModel
{
    public BookGenre BookGenre { get; set; }

    public string Title { get; set; }

    public BookGenreViewModel(BookGenre bookGenre, string title)
    {
        BookGenre = bookGenre;
        Title = title;
    }
}

/// <summary>
/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
///
/// Step 1a) Using this custom control in a XAML file that exists in the current project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:Fb2.Document.WPF.Playground.Components"
///
///
/// Step 1b) Using this custom control in a XAML file that exists in a different project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:Fb2.Document.WPF.Playground.Components;assembly=Fb2.Document.WPF.Playground.Components"
///
/// You will also need to add a project reference from the project where the XAML file lives
/// to this project and Rebuild to avoid compilation errors:
///
///     Right click on the target project in the Solution Explorer and
///     "Add Reference"->"Projects"->[Browse to and select this project]
///
///
/// Step 2)
/// Go ahead and use your control in the XAML file.
///
///     <MyNamespace:TitleRendererControl/>
///
/// </summary>
public class TitleRendererControl : Control
{
    private static Lazy<Dictionary<string, string>> validGenres = new(() => new Dictionary<string, string>
    {
        ["sf_history"] = "Alternate history",
        ["sf_action"] = "Action fiction",
        ["sf_epic"] = "Epic fantasy",
        ["sf_heroic"] = "Heroic fantasy",
        ["sf_detective"] = "Detective fiction",
        ["sf_cyberpunk"] = "Cyberpunk",
        ["sf_space"] = "Space opera",
        ["sf_social"] = "Social science fiction",
        ["sf_horror"] = "Horror fiction",
        ["sf_humor"] = "Comedy fiction",
        ["sf_fantasy"] = "Fantasy",
        ["sf"] = "Science fiction",
        ["det_classic"] = "Classic detective",
        ["det_police"] = "Police detective",
        ["det_action"] = "Action",
        ["det_irony"] = "Irony detective",
        ["det_history"] = "Historical detective",
        ["det_espionage"] = "Espionage",
        ["det_crime"] = "Criminal detectives",
        ["det_political"] = "Political detective",
        ["det_maniac"] = "Maniacs",
        ["det_hard"] = "Hard detective", // lol wut
        ["thriller"] = "Thriller",
        ["detective"] = "Detective",
        ["prose_classic"] = "Classic prose",
        ["prose_history"] = "Historical prose",
        ["prose_contemporary"] = "Contemporary/Modern prose",
        ["prose_counter"] = "Counter-culture",
        ["love_contemporary"] = "Modern love romance",
        ["love_history"] = "Historical love romance",
        ["love_detective"] = "Detective love romance",
        ["love_short"] = "Short love story",
        ["love_erotica"] = "Erotics",
        ["adv_western"] = "Western",
        ["adv_history"] = "Historical adventures",
        ["adv_indian"] = "Indian adventures",
        ["adv_maritime"] = "Maritime adventures",
        ["adv_geo"] = "Travels and geography",
        ["adv_animal"] = "Nature and animals",
        ["adventure"] = "Adventures",
        ["child_tale"] = "Fairytale",
        ["child_verse"] = "Children verse",
        ["child_prose"] = "Children prose",
        ["child_sf"] = "Children science fiction",
        ["child_det"] = "Children drama",
        ["child_adv"] = "Children adventures",
        ["child_education"] = "Children education",
        ["children"] = "Children literature",
        ["poetry"] = "Poetry",
        ["dramaturgy"] = "Dramaturgy",
        ["antique_ant"] = "Antique literature",
        ["antique_european"] = "Ancient Europe literature",
        ["antique_east"] = "Ancient East literature",
        ["antique_myths"] = "Myths, Legends and Epos",
        ["antique"] = "Ancient/Antique literature",
        ["sci_history"] = "History",
        ["sci_psychology"] = "Psychology",
        ["sci_culture"] = "Culturology",
        ["sci_religion"] = "Religious studies",
        ["sci_philosophy"] = "Philosophy",
        ["sci_politics"] = "Politics",
        ["sci_business"] = "Business",
        ["sci_juris"] = "Jurisdiction",
        ["sci_linguistic"] = "Linguistics",
        ["sci_medicine"] = "Medicine",
        ["sci_phys"] = "Physics",
        ["sci_math"] = "Mathematics",
        ["sci_chem"] = "Chemistry",
        ["sci_biology"] = "Biology",
        ["sci_tech"] = "Technical sciences",
        ["science"] = "Science",
        ["comp_www"] = "Internet and Web",
        ["comp_programming"] = "Computer programming",
        ["comp_hard"] = "Computer hardware",
        ["comp_soft"] = "Computer software",
        ["comp_db"] = "Databases",
        ["comp_osnet"] = "Operating Systems and Networks",
        ["computers"] = "Computers",
        ["ref_encyc"] = "Encyclopedia",
        ["ref_dict"] = "Dictionary",
        ["ref_ref"] = "Handbook",
        ["ref_guide"] = "Guide",
        ["reference"] = "Reference literature",
        ["nonf_biography"] = "Biographies and Memoirs",
        ["nonf_publicism"] = "Publicism",
        ["nonf_criticism"] = "Criticism",
        ["design"] = "Art and Design",
        ["nonfiction"] = "Documentary literature",
        ["religion_rel"] = "Religion",
        ["religion_esoterics"] = "Esoterics",
        ["religion_self"] = "Self-development",
        ["religion"] = "Theology",
        ["humor_anecdote"] = "Anecdotes",
        ["humor_prose"] = "Humorous prose",
        ["humor_verse"] = "Humorous poetry",
        ["humor"] = "Comedy",
        ["home_cooking"] = "Cooking",
        ["home_pets"] = "Pets",
        ["home_crafts"] = "Hobbies and crafts",
        ["home_entertain"] = "Entertainment",
        ["home_health"] = "Health",
        ["home_garden"] = "Garden",
        ["home_diy"] = "DIY",
        ["home_sport"] = "Sport",
        ["home_sex"] = "Erotic/Sexual literature",
        ["home"] = "Home"
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    public ObservableCollection<BookGenreViewModel> BookGenres { get; set; } = new();

    static TitleRendererControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(TitleRendererControl),
            new FrameworkPropertyMetadata(typeof(TitleRendererControl)));
    }

    private FlowDocument? TitleDoc = null;
    private StackPanel? GenresPanel = null;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        TitleDoc = GetTemplateChild("TitleDoc") as FlowDocument;
        GenresPanel = GetTemplateChild("GenresPanel") as StackPanel;
    }

    public TitleInfoBase TitleInfo
    {
        get { return (TitleInfoBase)GetValue(TitleInfoProperty); }
        set { SetValue(TitleInfoProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TitleInfo.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TitleInfoProperty =
        DependencyProperty.Register(
            "TitleInfo",
            typeof(TitleInfoBase),
            typeof(TitleRendererControl),
            new PropertyMetadata(null, new PropertyChangedCallback(OnTitleInfoPropertyChangedCallback)));

    private static void OnTitleInfoPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sender = d as TitleRendererControl;
        if (sender == null)
            return;

        var titleInfo = sender.TitleInfo;
        if (titleInfo == null)
        {
            return;
        }

        var authors = titleInfo.GetDescendants<Author>().Where(a =>
        {
            var result = a != null && a.HasContent &&
                ((a.TryGetFirstDescendant(ElementNames.FirstName, out var fName) && fName!.HasContent) ||
                 (a.TryGetFirstDescendant(ElementNames.MiddleName, out var mName) && mName!.HasContent) ||
                 (a.TryGetFirstDescendant(ElementNames.LastName, out var lName) && lName!.HasContent) ||
                 (a.TryGetFirstDescendant(ElementNames.NickName, out var nName) && nName!.HasContent));
            return result;
        });
        var titleInfoBookName = titleInfo.GetFirstDescendant<BookTitle>();
        var subTitle = titleInfo.GetFirstDescendant<SubTitle>();
        var annotation = titleInfo.GetFirstDescendant<Annotation>();
        //var sequences = titleInfo.GetDescendants<SequenceInfo>().Where(s => s != null && !s.IsEmpty && s.HasAttributes);
        var sequences = titleInfo.GetDescendants<SequenceInfo>().Where(s => s != null && s.HasContent && s.HasAttributes);
        var keywords = titleInfo.GetDescendants<Keywords>().Where(k => k != null && k.HasContent);

        var nodes = new List<Fb2Node>();

        nodes.AddRange(authors);
        nodes.AddRange(sequences);

        if (titleInfoBookName != null)
            nodes.Add(titleInfoBookName);

        if (subTitle != null)
            nodes.Add(subTitle);

        if (annotation != null)
            nodes.Add(annotation);

        nodes.AddRange(keywords.Where(k => k != null && k.HasContent));

        var mappedTitle = Fb2Mapper.Instance.MapNodes(nodes);
        var allTextElements = mappedTitle
            .SelectMany(c => c)
            .Where(c => c != null)
            .ToList();

        var validGenreSet = validGenres.Value;

        var genres = titleInfo.GetDescendants<BookGenre>()
            .Where(bg =>
            {
                if (!bg.HasContent)
                    return false;

                var genreText = bg.Content;
                var isValidGenre = validGenreSet.ContainsKey(genreText);
                return isValidGenre;
            })
            .Select(g => new BookGenreViewModel(g, validGenreSet[g.Content]));

        if (genres.Any())
        {
            foreach (var genre in genres)
            {
                sender.BookGenres.Add(genre);
            }
        }
        else
        {
            if (sender.GenresPanel != null)
                sender.GenresPanel.Visibility = Visibility.Collapsed;
        }

        var blockTextElements = Utils.Instance.Paragraphize(allTextElements);

        var doc = sender.FindName("TitleDoc");

        sender.TitleDoc?.Blocks.AddRange(blockTextElements);
    }
}
