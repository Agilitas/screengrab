using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fpil.WpfExtensions {
    public class FormLayoutGrid : Grid {

        private static readonly DependencyProperty PaddingProperty = DependencyProperty.Register("Padding",
                                                                            typeof(Thickness),
                                                                            typeof(FormLayoutGrid),
                                                                            new UIPropertyMetadata(new Thickness(0.0),
                                                                            OnPaddingChanged));

        private static readonly DependencyProperty ControlPaddingProperty = DependencyProperty.Register("ControlPadding",
                                                                    typeof(Thickness),
                                                                    typeof(FormLayoutGrid),
                                                                    new UIPropertyMetadata(new Thickness(0.0),
                                                                    OnControlPaddingChanged));

        public Thickness ControlPadding {
            get { return (Thickness)GetValue(ControlPaddingProperty); }
            set { SetValue(ControlPaddingProperty, value); }
        }

        private static void OnControlPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            FormLayoutGrid grid = d as FormLayoutGrid;
            if (grid != null) grid.UpdateLayout();
        }

        public Thickness Padding {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public FormLayoutGrid() {
            ControlPadding = new Thickness(0, 2, 10, 2);
            Loaded += FormLayoutGridLoaded;
        }

        void FormLayoutGridLoaded(object sender, RoutedEventArgs e) {
            //if there are no column definitions default to 2
            if (ColumnDefinitions.Count == 0) {
                ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }
            LayoutChildren();
        }

        private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            FormLayoutGrid grid = d as FormLayoutGrid;
            if (grid != null) grid.UpdateLayout();
        }

        private bool _childrenRearranged;

        void LayoutChildren() {

            if (_childrenRearranged) {
                return;
            }

            _childrenRearranged = true;

            int currentRow = 0;
            int currentCol = 1;

            int childControls = VisualTreeHelper.GetChildrenCount(this);

            if (Padding.Top > 0) {
                var row = new RowDefinition { Height = new GridLength(Padding.Top) };
                RowDefinitions.Add(row);
            }

            //redo this to add a border as the parent element???
            ColumnDefinitions.Insert(0, new ColumnDefinition { Width = new GridLength(Padding.Left) });
            ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Padding.Right) });

            int actualColumnCount = ColumnDefinitions.Count - 2;
            int columnsUsed = 0;
            var margin = new Thickness();
            bool isFirstColumn = true;

            for (int idx = 0; idx < childControls; idx++) {

                var ctrl = VisualTreeHelper.GetChild(this, idx) as FrameworkElement;
                if (ctrl == null) {
                    throw new ArgumentException("ctrl is null!!!");
                }

                int columnProperty = GetColumn(ctrl);           //this will be 0 if not set, however 1 column will still be used
                int columnSpanProperty = GetColumnSpan(ctrl);   //this will be 1 if not set

                int columnsTakenByControl = (columnProperty == 0 ? 1 : columnProperty + 1) + (columnSpanProperty - 1);

                bool isLastColumn = ((columnsUsed + columnsTakenByControl) % actualColumnCount) == 0;

                if (isFirstColumn) {
                    //ctrl.SetValue(BackgroundProperty, Brushes.Green);
                    //first column - reset the margin. add a new row, inc the row count and reset the column to 1
                    margin = new Thickness(ControlPadding.Left, ControlPadding.Top, ControlPadding.Right,
                                               ControlPadding.Bottom);
                    var row = new RowDefinition { Height = GridLength.Auto };
                    RowDefinitions.Add(row);

                    currentRow++;
                    currentCol = 1;
                }
                if (isLastColumn) {
                    margin.Right = 0;
                }

                columnsUsed += columnsTakenByControl;
                isFirstColumn = (columnsUsed % actualColumnCount) == 0;

                //handle manually set column
                ctrl.Margin = margin;
                if (ctrl.VerticalAlignment == VerticalAlignment.Stretch) {
                    ctrl.VerticalAlignment = VerticalAlignment.Center;
                }

                //if the Grid.Column property is set...
                if (columnProperty > 0) {
                    currentCol = columnProperty + 1;
                }

                SetColumnSpan(ctrl, columnSpanProperty);
                SetColumn(ctrl, currentCol);
                SetRow(ctrl, currentRow);

                currentCol += columnSpanProperty;

            }

            if (Padding.Bottom > 0) {
                var row = new RowDefinition { Height = new GridLength(Padding.Bottom) };
                RowDefinitions.Add(row);
            }

        }

    }
}
