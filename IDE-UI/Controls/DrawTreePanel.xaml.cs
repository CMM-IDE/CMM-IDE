
using Microsoft.Msagl.Drawing;
using System;
using System.Windows.Controls;


namespace IDE_UI.Controls
{
    /// <summary>
    /// DrawTreePanel.xaml 的交互逻辑
    /// </summary>
    public partial class DrawTreePanel : UserControl
    {

        public DrawTreePanel()
        {
            InitializeComponent();
        }

        public Graph Graph {
            get {
                return graph;
            }
            set {
                graph = value;
                graphControl.Graph = null;
                graphControl.UpdateLayout();
                graphControl.Graph = value;
            }
        }
        private Graph graph;

        public void setNeedUpdate()
        {
            graphControl.UpdateLayout();
        }
    }
}
