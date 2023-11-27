using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

//to-do: scrollbar arrows

namespace TinyGiantStudio.Text.EditorFiles
{
    /// <summary>
    /// This provides a uniform way for all UI in Modular 3D Text to switch themes in UIToolkit
    /// </summary>
    public class EditorThemeManager
    {
        #region Variable

        #region Variable declarations

        #region XML classNames

        private readonly string normalButtonClassName = "normalButton";
        private readonly string iconButtonClassName = "iconButton";
        private readonly string textButtonClassName = "textButton";

        #endregion XML classNames

        private readonly string themeEditorPrefName = "TGSInspectorDarkTheme";
        public Theme _theme;

        public Theme Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                UpdateTheme();
            }
        }

        private VisualElement _root;

        public VisualElement Root
        {
            get { return _root; }
            set
            {
                _root = value;
                UpdateReferences();
            }
        }

        private UnityEngine.UIElements.Button darkModeButton;
        private UnityEngine.UIElements.Button lightModeButton;
        private VisualElement themeModeBackgroundButton;
        private VisualElement iconBackground;
        private VisualElement lightModeBG;
        private VisualElement darkModeBG;

        #endregion Variable declarations

        private void UpdateReferences()
        {
            darkModeButton = Root.Q<UnityEngine.UIElements.Button>("DarkModeButton");
            darkModeButton.clicked += () => Theme = Theme.Light;

            lightModeButton = Root.Q<UnityEngine.UIElements.Button>("LightModeButton");
            lightModeButton.clicked += () => Theme = Theme.Dark;

            themeModeBackgroundButton = Root.Q<VisualElement>("ThemeColor");
            iconBackground = Root.Q<VisualElement>("IconBackground");

            lightModeBG = Root.Q<VisualElement>("LightModeBG");
            darkModeBG = Root.Q<VisualElement>("DarkModeBG");
        }

        #endregion Variable



        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorThemeManager"/> class.
        /// </summary>
        /// <param name="newContainer">The root of the UI.</param>
        public EditorThemeManager(GroupBox newContainer)
        {
            Root = newContainer;
        }

        public void Start()
        {
            //if (EditorGUIUtility.isProSkin)
            Theme = GetTheme();

            AddFoldoutAnimations();
        }

        public void UpdateTheme()
        {
            UpdateThemeSwitcherButtons();

            ChangeUIBackground();
            ChangeAllButtonColor();
            ChangeAllIconButtonColor();
            ChangeAllTextButtonColor();
            ChangeAllLabelColor();
            ChangeAllFieldBackgroundColor();
            ChangeFoldoutIconColor();
        }

        #endregion Public Methods

        private void AddFoldoutAnimations()
        {
            float yMoveAmount = -20;

            List<Foldout> foldouts = Root.Query<Foldout>().ToList();
            foreach (Foldout foldout in foldouts)
            {
                var contentContainer = foldout.Q<VisualElement>("unity-content");
                if (foldout.value)
                {
                    contentContainer.style.opacity = 1;
                    contentContainer.style.translate = new StyleTranslate(new Translate(0, 0));
                }
                else
                {
                    contentContainer.style.opacity = 0;
                    contentContainer.style.translate = new StyleTranslate(new Translate(0, yMoveAmount));
                }

                foldout.RegisterValueChangedCallback(ev =>
                {
                    if (ev.target != foldout) //Any toggle inside the foldout also triggers this value change callback. This makes sure the value was changed for this
                        return;

                    if (ev.newValue)
                    {
                        contentContainer.style.opacity = 1;
                        contentContainer.style.translate = new StyleTranslate(new Translate(0, 0));
                        //contentContainer.style.display = DisplayStyle.Flex;
                    }
                    else
                    {
                        contentContainer.style.opacity = 0;
                        contentContainer.style.translate = new StyleTranslate(new Translate(0, yMoveAmount));
                        //contentContainer.style.display = DisplayStyle.None;
                    }
                });
            }
        }

        private void UpdateThemeSwitcherButtons()
        {
            if (Theme == Theme.Light)
            {
                EditorPrefs.SetBool(themeEditorPrefName, false);

                themeModeBackgroundButton.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 1f);

                lightModeButton.BringToFront();

                darkModeButton.style.opacity = 0;
                darkModeBG.style.opacity = 0;
                lightModeButton.style.opacity = 1;
                lightModeBG.style.opacity = 1;

                iconBackground.style.translate = new StyleTranslate(new Translate(10, 0));
                darkModeButton.style.translate = new StyleTranslate(new Translate(10, 0));
                lightModeButton.style.translate = new StyleTranslate(new Translate(10, 0));
            }
            else
            {
                EditorPrefs.SetBool(themeEditorPrefName, true);

                ColorUtility.TryParseHtmlString("#222222", out Color themeModeBackgroundButtonBackgroundColor);
                themeModeBackgroundButton.style.backgroundColor = themeModeBackgroundButtonBackgroundColor;

                darkModeButton.BringToFront();

                darkModeButton.style.opacity = 1;
                darkModeBG.style.opacity = 1;
                lightModeButton.style.opacity = 0;
                lightModeBG.style.opacity = 0;

                iconBackground.style.translate = new StyleTranslate(new Translate(-10, 0));
                darkModeButton.style.translate = new StyleTranslate(new Translate(-10, 0));
                lightModeButton.style.translate = new StyleTranslate(new Translate(-10, 0));
            }
        }

        private void ChangeUIBackground()
        {
            Color backgroundColor;
            if (Theme == Theme.Light)
                ColorUtility.TryParseHtmlString("#AFB8BC", out backgroundColor);
            else
                ColorUtility.TryParseHtmlString("#313030", out backgroundColor);

            Root.Q<GroupBox>("RootHolder").style.unityBackgroundImageTintColor = backgroundColor;
        }

        private void ChangeAllButtonColor()
        {
            List<UnityEngine.UIElements.Button> normalButtons = Root.Query<UnityEngine.UIElements.Button>(className: normalButtonClassName).ToList();
            foreach (UnityEngine.UIElements.Button normalButton in normalButtons)
            {
                if (Theme == Theme.Light)
                {
                    normalButton.RemoveFromClassList("BlackText");
                    normalButton.AddToClassList("WhiteText");
                }
                else
                {
                    normalButton.RemoveFromClassList("WhiteText");
                    normalButton.AddToClassList("BlackText");
                }
            }
        }

        private void ChangeAllIconButtonColor()
        {
            List<UnityEngine.UIElements.Button> iconButtons = Root.Query<UnityEngine.UIElements.Button>(className: iconButtonClassName).ToList();
            foreach (UnityEngine.UIElements.Button iconButton in iconButtons)
            {
                if (Theme == Theme.Light)
                {
                    iconButton.style.unityBackgroundImageTintColor = new Color(0.25f, 0.25f, 0.25f, 1f);
                }
                else
                {
                    iconButton.style.unityBackgroundImageTintColor = new Color(0.8f, 0.8f, 0.8f);
                }
            }
        }

        private void ChangeAllTextButtonColor()
        {
            List<UnityEngine.UIElements.Button> textButtons = Root.Query<UnityEngine.UIElements.Button>(className: textButtonClassName).ToList();
            foreach (UnityEngine.UIElements.Button textButton in textButtons)
            {
                if (Theme == Theme.Light)
                {
                    textButton.RemoveFromClassList("darkThemeLinkText");
                    textButton.AddToClassList("lightThemeLinkText");
                }
                else
                {
                    textButton.RemoveFromClassList("lightThemeLinkText");
                    textButton.AddToClassList("darkThemeLinkText");
                }
            }
        }

        private void ChangeAllLabelColor()
        {
            Color color;
            if (Theme == Theme.Light)
                color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            else
                color = new Color(0.9f, 0.9f, 0.8f, 0.7f);

            List<Label> labels = Root.Query<Label>(className: "unity-label").ToList();
            foreach (Label label in labels)
            {
                label.style.color = color;
            }
        }

        private void ChangeAllFieldBackgroundColor()
        {
            Color backgroundColor;
            Color color2;
            if (Theme == Theme.Light)
            {
                backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.19f);
                color2 = Color.black;
            }
            else
            {
                backgroundColor = new Color(0.8f, 0.825f, 0.8f, 0.7f);
                color2 = Color.black;
            }

            List<VisualElement> backgrounds = Root.Query<VisualElement>(className: "unity-base-text-field__input").ToList();
            foreach (VisualElement background in backgrounds)
            {
                background.style.backgroundColor = backgroundColor;
                background.style.color = color2;
            }
        }

        private void ChangeFoldoutIconColor()
        {
            Color color;
            if (Theme == Theme.Light)
                color = new Color(0.0f, 0.025f, 0.05f, 0.9f);
            else
                color = new Color(0.81f, 0.81f, 0.81f, 0.9f);


            List<VisualElement> toggleCheckMarks = Root.Query<VisualElement>(className: "unity-foldout__checkmark").ToList();
            foreach (VisualElement toggleCheckMark in toggleCheckMarks)
            {
                toggleCheckMark.style.unityBackgroundImageTintColor = color;
            }
        }

        private Theme GetTheme()
        {
            if (EditorPrefs.GetBool(themeEditorPrefName, EditorGUIUtility.isProSkin))
                return Theme.Dark;
            else
                return Theme.Light;
        }
    }

    public enum Theme
    {
        Dark,
        Light
    }
}