using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Linq;

namespace Translator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Транслятор из ERM-модели в UML-модель.\nАнастасия М. Данейко\nФакультет информатики\n2013 год",
                "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOpenErm_Click(object sender, EventArgs e)
        {
            String fileName = "";
            openFileDialog1.Filter = "ERM-model file (*.ermmdsl)|*.ermmdsl|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                edtErmName.Text = fileName;
            }
        }

        private void btnSaveUml_Click(object sender, EventArgs e)
        {
            String fileName = "";
            saveFileDialog1.Filter = "UML-model file (*.uml)|*.uml|All Files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = "";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog1.FileName;
                edtUmlName.Text = fileName;
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnOpenErm_Click(sender, e);
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnSaveUml_Click(sender, e);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {
                XslCompiledTransform xsl = new XslCompiledTransform();
                xsl.Load(xsltTable);
                modifyErm(edtErmName.Text);
                xsl.Transform(edtErmName.Text + ".tmp", edtUmlName.Text);
                XmlDocument doc = new XmlDocument();
                doc.Load(edtUmlName.Text);
                XmlNode root = doc.SelectNodes("/*[name()='modelRoot']")[0];
                XmlAttribute[] a = new XmlAttribute[root.Attributes.Count];
                root.Attributes.CopyTo(a, 0);
                root.Attributes.RemoveAll();
                doc.Save(edtUmlName.Text);
                transformRelationship(edtErmName.Text + ".tmp", edtUmlName.Text);
                foreach (XmlAttribute at in a)
                    root.Attributes.Append(at);
                doc.Save(edtUmlName.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            File.Delete(edtErmName.Text + ".tmp");
            Close();
        }

        private void modifyErm(string p)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(p);
            XmlNode node = doc.SelectNodes("/*[name()='ermModel']")[0];
            node.Attributes.RemoveAll();
            doc.Save(p + ".tmp");
        }


        private void transformRelationship(String ermFile, String umlFile)
        {
            XmlDocument erm = new XmlDocument();
            XmlDocument uml = new XmlDocument();
            erm.Load(ermFile);
            uml.Load(umlFile);
            XmlNodeList relationList = erm.SelectNodes("/ermModel/relationshipSet/ermModelHasRelationshipSet/relationshipSet");
            foreach (XmlNode relation in relationList)
            {
                String relationName = relation.Attributes["name"].Value;
                List<String> entitySets = new List<String>();
                List<String> roles = new List<String>();
                List<String> cards = new List<String>();
                List<String> attrNames = new List<String>();
                List<String> attrTypes = new List<String>();
                XmlNodeList attrList = relation.SelectNodes("valueSet/relationshipSetReferencesValueSet");
                foreach (XmlNode attr in attrList)
                {
                    attrNames.Add(attr.Attributes["name"].Value);
                    String t = attr.SelectNodes("valueSetMoniker")[0].Attributes["name"].Value;
                    t = t.Remove(0, t.LastIndexOf('/') + 1);
                    attrTypes.Add(t);
                }
                XmlNodeList entitySetList = erm.SelectNodes("/ermModel/elements/entitySet");
                foreach (XmlNode entitySet in entitySetList)
                {
                    String entitySetName = entitySet.Attributes["name"].Value;
                    XmlNodeList roleSet = entitySet.SelectNodes("relationshipSet/entitySetPlaysRoleInRelationshipSet");
                    foreach (XmlNode role in roleSet)
                    {
                        String roleName = role.SelectNodes("relationshipSetMoniker")[0].Attributes["name"].Value;
                        if (!roleName.Contains(relationName)) continue;
                        String c = role.Attributes["card"].Value;
                        String r = role.Attributes["name"].Value;
                        entitySets.Add(entitySetName);
                        cards.Add(c);
                        roles.Add(r);
                        break;
                    }
                }

                if (entitySets.Count == 2 && attrNames.Count == 0)
                {
                    //находим uml, ему ставим отношение
                    XmlNode node = uml.SelectSingleNode("/modelRoot/types/modelClass[@name='" + entitySets[0] + "']/bidirectionalTargets");
                    XmlDocument temp = new XmlDocument();
                    temp.LoadXml("<bidirectionalAssociation sourceRoleName=\"" + roles[0] + "\" targetRoleName=\"" + roles[1] + "\"><modelClassMoniker name=\"//" + entitySets[1] + "\"/></bidirectionalAssociation>");
                    XmlNode child = node.OwnerDocument.ImportNode(temp.DocumentElement, true);
                    node.AppendChild(child);
                }
                else
                {
                    //создаем новый класс, и ему ставим атрибуты и прочую фигню.
                }


            }
        }

        private const String umlString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        private const String xsltTable = "Z:\\Dropbox\\We\\Translator\\Samples\\Translation.xsl";
    }
}
