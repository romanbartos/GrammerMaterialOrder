using GrammerMaterialOrder.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static GrammerMaterialOrder.MVVM.ViewModels.SpravceViewModel;

namespace GrammerMaterialOrder.MVVM.Views
{
    /// <summary>
    /// Interakční logika pro PoznamkaSkladnikView.xaml
    /// </summary>
    public partial class PoznamkaSkladnikView : Window
    {
        private int objednavkaId_;
        public PoznamkaSkladnikView(int objednavkaId)
        {
            InitializeComponent();
            objednavkaId_ = objednavkaId;
            //PoznamkaSkladnikRtb.AppendText = MyString;

            //PoznamkaSkladnikRtb.Document.Blocks.Clear();
            //PoznamkaSkladnikRtb.Document.Blocks.Add(new Paragraph(new Run(MyString)));
        }

        private void UlozitPoznamkaSkladnik_Click(object sender, RoutedEventArgs e)
        {
            string richText = new TextRange(PoznamkaSkladnikRtb.Document.ContentStart, PoznamkaSkladnikRtb.Document.ContentEnd).Text;
            if (string.IsNullOrEmpty(richText))
            {
                MessageBox.Show(@"Nebyla vyplněna poznámka.", @"Poznámka", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            using MaterialOrderContext db = new();

            //ProdOrderEmployeePlan prodOrderEmployeePlan = new()
            //{
            //    Note = richText
            //};
            //_ = db.ProdOrdersEmployeePlan.Add(prodOrderEmployeePlan);
            //_ = db.SaveChanges();

            var poznamka = db.ProdOrdersEmployeePlan.Where(p => p.Id == objednavkaId_).First();
            poznamka.Note = richText;

            // uloží poznámku pro vybranou zakázku
            _ = db.SaveChanges();
        }
    }
}
