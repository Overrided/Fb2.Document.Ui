using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Fb2.Document.Constants;
using Fb2.Document.Models;
using Fb2.Document.Models.Base;
using Fb2.Document.UWP.Playground.Common;
using RichTextView.UWP.DTOs;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fb2.Document.UWP.Playground.Controls
{

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

    public class TitleInfoBaseRendererViewModel : ObservableObject
    {
        private RichContent titleInfoContent;

        public RichContent TitleInfoContent
        {
            get { return titleInfoContent; }
            set
            {
                OnPropertyChanging();
                titleInfoContent = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BookGenreViewModel> BookGenres { get; set; } = new ObservableCollection<BookGenreViewModel>();
    }

    public sealed class TitleInfoBaseRenderer : Control
    {
        private static Lazy<Dictionary<string, string>> validGenres = new Lazy<Dictionary<string, string>>(() => new Dictionary<string, string>
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

        public TitleInfoBaseRendererViewModel ViewModel { get; set; }

        public TitleInfoBaseRenderer()
        {
            this.DefaultStyleKey = typeof(TitleInfoBaseRenderer);
            ViewModel = new TitleInfoBaseRendererViewModel();
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public TitleInfoBase TitleInfo
        {
            get { return (TitleInfoBase)GetValue(TitleInfoProperty); }
            set { SetValue(TitleInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleInfoProperty =
            DependencyProperty.Register(
                nameof(TitleInfo),
                typeof(TitleInfoBase),
                typeof(TitleInfoBaseRenderer),
                new PropertyMetadata(null, new PropertyChangedCallback(OnTitleInfoProperyChangedCallback)));

        private static void OnTitleInfoProperyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as TitleInfoBaseRenderer;
            if (sender == null)
            {
                return;
            }

            var titleInfo = sender.TitleInfo;
            if (titleInfo == null)
            {
                return;
            }

            var authors = titleInfo.GetDescendants<Author>().Where(a =>
            {
                var result = a != null && !a.IsEmpty &&
                    ((a.TryGetFirstDescendant(ElementNames.FirstName, out var fName) && !fName.IsEmpty) ||
                     (a.TryGetFirstDescendant(ElementNames.MiddleName, out var mName) && !mName.IsEmpty) ||
                     (a.TryGetFirstDescendant(ElementNames.LastName, out var lName) && !lName.IsEmpty) ||
                     (a.TryGetFirstDescendant(ElementNames.NickName, out var nName) && !nName.IsEmpty));
                return result;
            });
            var titleInfoBookName = titleInfo.GetFirstDescendant<BookTitle>();
            var subTitle = titleInfo.GetFirstDescendant<SubTitle>();
            var annotation = titleInfo.GetFirstDescendant<Annotation>();
            //var sequences = titleInfo.GetDescendants<SequenceInfo>().Where(s => s != null && !s.IsEmpty && s.HasAttributes);
            var sequences = titleInfo.GetDescendants<SequenceInfo>().Where(s => s != null && s.Attributes.Count != 0);
            var keywords = titleInfo.GetDescendants<Keywords>().Where(k => k != null && !k.IsEmpty);

            var nodes = new List<Fb2Node>();

            nodes.AddRange(authors);
            nodes.AddRange(sequences);

            if (titleInfoBookName != null)
                nodes.Add(titleInfoBookName);

            if (subTitle != null)
                nodes.Add(subTitle);

            if (annotation != null)
                nodes.Add(annotation);

            nodes.AddRange(keywords.Where(k => k != null && !k.IsEmpty));

            var mappedNodes = Fb2Mapper.Instance.MapNodes(nodes, Size.Empty);

            var normalizedContent = mappedNodes.SelectMany(uic => uic);

            var contentPage = new RichContentPage(normalizedContent);
            var content = new RichContent(new List<RichContentPage>(1) { contentPage });
            sender.ViewModel.TitleInfoContent = content;

            var validGenreSet = validGenres.Value;

            var genres = titleInfo.GetDescendants<BookGenre>()
                .Where(bg =>
                {
                    if (bg.IsEmpty)
                        return false;

                    var genreText = bg.Content;
                    var isValidGenre = validGenreSet.ContainsKey(genreText);
                    return isValidGenre;
                })
                .Select(g => new BookGenreViewModel(g, validGenreSet[g.Content]));
            foreach (var genre in genres)
            {
                sender.ViewModel.BookGenres.Add(genre);
            }
        }
    }

}
