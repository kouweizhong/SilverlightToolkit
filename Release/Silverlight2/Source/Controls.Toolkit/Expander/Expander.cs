﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that displays a header and has a collapsible 
    /// content window.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the content of the expanded window is too large for the window, 
    /// you can wrap the content of the Expander in a ScrollViewer control to 
    /// provide scrollable content. Scrolling capability is not automatically 
    /// provided by the Expander control. 
    /// </para>
    /// <para>
    /// For an Expander to work correctly, do not specify a Height on the 
    /// Expander control when the ExpandDirection property is set to Down or 
    /// Up. Similarly, do not specify a Width on the Expander control when 
    /// the ExpandDirection property is set to Left or Right. When you set a 
    /// size on the Expander control in the direction that the expanded content 
    /// is displayed, the area that is defined by the size parameter is 
    /// displayed with a border around it. This area displays even when the 
    /// window is collapsed. To set the size of the expanded window, set size 
    /// dimensions on the content of the Expander control or the ScrollViewer 
    /// that encloses the content.
    /// </para>
    /// <para>
    /// When an Expander control is the last element in a DockPanel, the 
    /// Expander is sized to fill the remaining area of the DockPanel. 
    /// To prevent this, set the LastChildFill property on the DockPanel to
    /// false, or make sure that the Expander is not the last element in a 
    /// DockPanel.
    /// </para>
    /// <para>
    ///  The alignment of Expander content can be defined by setting the 
    /// HorizontalContentAlignment and VerticalContentAlignment properties 
    /// on the control. These properties are applied to the header and to 
    /// the contents of the expanded window.
    /// </para>
    /// <para>
    /// Content Model: Expander is a HeaderedContentControl. Its content 
    /// property is Content and its header property is Header.
    /// </para>
    /// </remarks>
    /// <QualityBand>Stable</QualityBand>
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StatePressed, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]

    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]

    [TemplateVisualState(Name = VisualStates.StateExpanded, GroupName = VisualStates.GroupExpansion)]
    [TemplateVisualState(Name = VisualStates.StateCollapsed, GroupName = VisualStates.GroupExpansion)]

    [TemplateVisualState(Name = VisualStates.StateExpandDown, GroupName = VisualStates.GroupExpandDirection)]
    [TemplateVisualState(Name = VisualStates.StateExpandUp, GroupName = VisualStates.GroupExpandDirection)]
    [TemplateVisualState(Name = VisualStates.StateExpandLeft, GroupName = VisualStates.GroupExpandDirection)]
    [TemplateVisualState(Name = VisualStates.StateExpandRight, GroupName = VisualStates.GroupExpandDirection)]

    [TemplatePart(Name = Expander.ElementExpanderButtonName, Type = typeof(ToggleButton))]
    public class Expander : HeaderedContentControl, IUpdateVisualState
    {
        /// <summary>
        /// The name of the ExpanderButton template part.
        /// </summary>
        private const string ElementExpanderButtonName = "ExpanderButton";

        /// <summary>
        /// The ExpanderButton template part is a templated ToggleButton that's used 
        /// to expand and collapse the ExpandSite, which hosts the content.
        /// </summary>
        private ToggleButton _expanderButton;

        /// <summary>
        /// Gets or sets the ExpanderButton template part.
        /// </summary>
        private ToggleButton ExpanderButton
        {
            get { return _expanderButton; }
            set
            {
                // Detach from old ExpanderButton
                if (_expanderButton != null)
                {
                    _expanderButton.Click -= OnExpanderButtonClicked;
                }

                _expanderButton = value;

                if (_expanderButton != null)
                {
                    _expanderButton.IsChecked = IsExpanded;
                    _expanderButton.Click += OnExpanderButtonClicked;
                }
            }
        }

        #region public ExpandDirection ExpandDirection
        /// <summary>
        /// Gets or sets the direction in which the Expander content window opens.
        /// </summary>
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the ExpandDirection dependency property. 
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
                DependencyProperty.Register(
                        "ExpandDirection",
                        typeof(ExpandDirection),
                        typeof(Expander),
                        new PropertyMetadata(ExpandDirection.Down, OnExpandDirectionPropertyChanged));

        /// <summary>
        /// ExpandDirectionProperty PropertyChangedCallback call back static function.
        /// This function validates the new value before calling virtual function OnExpandDirectionChanged.
        /// </summary>
        /// <param name="d">Expander object whose ExpandDirection property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnExpandDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Expander ctrl = (Expander)d;
            ExpandDirection oldValue = (ExpandDirection)e.OldValue;
            ExpandDirection newValue = (ExpandDirection)e.NewValue;

            if (!IsValidExpandDirection(newValue))
            {
                ctrl.ExpandDirection = oldValue;

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.Expander_OnExpandDirectionPropertyChanged_InvalidValue,
                    newValue);
                throw new ArgumentException(message, "e");
            }

            ctrl.UpdateVisualState(true);
        }

        /// <summary>
        /// Check whether the passed in value o is a valid ExpandDirection enum value.
        /// </summary>
        /// <param name="o">The value to be checked.</param>
        /// <returns>True if o is a valid ExpandDirection enum value, false o/w.</returns>
        private static bool IsValidExpandDirection(object o)
        {
            ExpandDirection value = (ExpandDirection)o;

            return (value == ExpandDirection.Down ||
                    value == ExpandDirection.Left ||
                    value == ExpandDirection.Right ||
                    value == ExpandDirection.Up);
        }
        #endregion public ExpandDirection ExpandDirection

        #region public bool IsExpanded
        /// <summary>
        /// Gets or sets a value indicating whether the Expander content 
        /// window is visible.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsExpanded dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
                DependencyProperty.Register(
                        "IsExpanded",
                        typeof(bool),
                        typeof(Expander),
                        new PropertyMetadata(OnIsExpandedPropertyChanged));

        /// <summary>
        /// ExpandedProperty PropertyChangedCallback static function.
        /// </summary>
        /// <param name="d">Expander object whose Expanded property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Expander ctrl = (Expander)d;
            bool isExpanded = (bool)e.NewValue;

            // Notify any automation peers of the expansion change
            ExpanderAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(ctrl) as ExpanderAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent((bool)e.OldValue, isExpanded);
            }

            if (isExpanded)
            {
                ctrl.OnExpanded();
            }
            else
            {
                ctrl.OnCollapsed();
            }
        }
        #endregion public bool IsExpanded

        /// <summary>
        /// Occurs when the content window of an Expander control opens to 
        /// display both its header and content.
        /// </summary>
        public event RoutedEventHandler Expanded;

        /// <summary>
        /// Occurs when the content window of an Expander control closes and 
        /// only the Header is visible.
        /// </summary>
        public event RoutedEventHandler Collapsed;

        /// <summary>
        /// Initializes a new instance of the Expander class.
        /// </summary>
        public Expander()
        {
            DefaultStyleKey = typeof(Expander);
            Interaction = new InteractionHelper(this);
        }

        /// <summary>
        /// Builds the visual tree for the Expander control when a new 
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ExpanderButton = GetTemplateChild(ElementExpanderButtonName) as ToggleButton;
            Interaction.OnApplyTemplateBase();
        }

        /// <summary>
        /// Returns a ExpanderAutomationPeer for use by the Silverlight
        /// automation infrastructure.
        /// </summary>
        /// <returns>A ExpanderAutomationPeer object for the Expander.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ExpanderAutomationPeer(this);
        }

        /// <summary>
        /// Provides handling for the KeyDown event.
        /// </summary>
        /// <param name="e">Key event args.</param>
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled || !IsEnabled)
            {
                return;
            }

            bool isExpanded = IsExpanded;
            switch (ExpandDirection)
            {
                case ExpandDirection.Down:
                    if ((isExpanded && e.Key == Key.Up) || (!isExpanded && e.Key == Key.Down))
                    {
                        IsExpanded = !isExpanded;
                    }
                    break;
                case ExpandDirection.Up:
                    if ((isExpanded && e.Key == Key.Down) || (!isExpanded && e.Key == Key.Up))
                    {
                        IsExpanded = !isExpanded;
                    }
                    break;
                case ExpandDirection.Left:
                    if ((isExpanded && e.Key == Key.Right) || (!isExpanded && e.Key == Key.Left))
                    {
                        IsExpanded = !isExpanded;
                    }
                    break;
                case ExpandDirection.Right:
                    if ((isExpanded && e.Key == Key.Left) || (!isExpanded && e.Key == Key.Right))
                    {
                        IsExpanded = !isExpanded;
                    }
                    break;
            }
        }

        /// <summary>
        /// Raises the Expanded event when the IsExpanded property changes 
        /// from false to true.
        /// </summary>
        protected virtual void OnExpanded()
        {
            ToggleExpanded(Expanded, new RoutedEventArgs());
        }

        /// <summary>
        /// Raises the Collapsed event when the IsExpanded property changes 
        /// from true to false.
        /// </summary>
        protected virtual void OnCollapsed()
        {
            ToggleExpanded(Collapsed, new RoutedEventArgs());
        }

        /// <summary>
        /// Handle changes to the IsExpanded property.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="args">Event arguments.</param>
        private void ToggleExpanded(RoutedEventHandler handler, RoutedEventArgs args)
        {
            ToggleButton expander = ExpanderButton;
            if (expander != null)
            {
                expander.IsChecked = IsExpanded;
            }

            UpdateVisualState(true);
            RaiseEvent(handler, args);
        }

        /// <summary>
        /// Raise a RoutedEvent.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        /// <param name="args">Event arguments.</param>
        private void RaiseEvent(RoutedEventHandler handler, RoutedEventArgs args)
        {
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Handle ExpanderButton's click event.
        /// </summary>
        /// <param name="sender">The ExpanderButton in template.</param>
        /// <param name="e">Routed event arg.</param>
        private void OnExpanderButtonClicked(object sender, RoutedEventArgs e)
        {
            IsExpanded = !IsExpanded;
        }

        /// <summary>
        /// Gets or sets the helper that provides all of the standard
        /// interaction functionality.
        /// </summary>
        private InteractionHelper Interaction { get; set; }

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            UpdateVisualState(useTransitions);
        }

        /// <summary>
        /// Update the current visual state of the button.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal virtual void UpdateVisualState(bool useTransitions)
        {
            if (IsExpanded)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateExpanded);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateCollapsed);
            }

            switch (ExpandDirection)
            {
                case ExpandDirection.Down:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateExpandDown);
                    break;

                case ExpandDirection.Up:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateExpandUp);
                    break;

                case ExpandDirection.Left:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateExpandLeft);
                    break;

                default:
                    VisualStates.GoToState(this, useTransitions, VisualStates.StateExpandRight);
                    break;
            }

            // Handle the Common and Focused states
            Interaction.UpdateVisualStateBase(useTransitions);
        }
    }
}