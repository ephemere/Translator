﻿using System;
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
using System.Xml.Serialization;

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
                if (isAttr) xsl.Load(xsltTable1);
                if (isClass) xsl.Load(xsltTable2);
                modifyErm(edtErmName.Text);
                xsl.Transform(edtErmName.Text + ".tmp", edtUmlName.Text);
                XmlDocument doc = new XmlDocument();
                XmlTextReader r = new XmlTextReader(edtUmlName.Text);
                r.Namespaces = false;
                doc.Load(r);
                r.Close();
                XmlNode root = doc.SelectSingleNode("/*[name()='modelRoot']");
                root.Attributes.RemoveAll();
                doc.Save(edtUmlName.Text + ".tmp");
                transformRelationship(edtErmName.Text + ".tmp", edtUmlName.Text + ".tmp");
                doc.Load(edtUmlName.Text + ".tmp");
                root = doc.SelectSingleNode("/*[name()='modelRoot']");
                {
                    XmlDocument d = new XmlDocument();
                    d.Load(edtUmlName.Text);
                    XmlNode dr = d.SelectSingleNode("/*[name()='modelRoot']");
                    XmlAttribute[] attrs = new XmlAttribute[dr.Attributes.Count];
                    dr.Attributes.CopyTo(attrs, 0);

                    foreach (XmlAttribute a in attrs)
                    {
                        XmlAttribute at = doc.CreateAttribute(a.Name);
                        at.Value = a.Value;
                        root.Attributes.Append(at);
                    }
                    doc.Save(edtUmlName.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            File.Delete(edtErmName.Text + ".tmp");
            File.Delete(edtUmlName.Text + ".tmp");
            Close();
        }

        private void modifyErm(string p)
        {
            XmlDocument doc = new XmlDocument();
            XmlTextReader r = new XmlTextReader(p);
            r.Namespaces = false;
            doc.Load(r);
            doc.SelectSingleNode("/*[name()='ermModel']").Attributes.RemoveAll();
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
                    String t = "";
                    if (attr.Attributes["name"] != null)
                        t = attr.Attributes["name"].Value;
                    attrNames.Add(t);
                    if (attr.SelectNodes("valueSetMoniker")[0].Attributes["name"] != null)
                        t = attr.SelectNodes("valueSetMoniker")[0].Attributes["name"].Value;
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
                        String c = "";
                        if (role.Attributes["card"] != null)
                            c = role.Attributes["card"].Value;
                        String r = "";
                        if (role.Attributes["name"] != null)
                            r = role.Attributes["name"].Value;
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
                    if (node == null)
                    {
                        node = uml.SelectSingleNode("/modelRoot/types/modelClass[@name='" + entitySets[0] + "']");
                        AddChild("<bidirectionalTargets/>", node);
                        node = uml.SelectSingleNode("/modelRoot/types/modelClass[@name='" + entitySets[0] + "']/bidirectionalTargets");
                    }
                    String sourceMultiplicity = "ZeroMany";
                    String targetMultiplicity = "ZeroMany";
                    if (cards[0] == "1") sourceMultiplicity = "ZeroOne";
                    if (cards[1] == "1") targetMultiplicity = "ZeroMany";
                    AddChild("<bidirectionalAssociation sourceMultiplicity=\"" + sourceMultiplicity + "\" targetMultiplicity=\"" + targetMultiplicity + "\" sourceRoleName=\"" + roles[0] + "\" targetRoleName=\"" + roles[1] + "\"><modelClassMoniker name=\"//" + entitySets[1] + "\"/></bidirectionalAssociation>", node);
                }
                else
                {
                    //создаем новый класс, и ему ставим атрибуты и прочую фигню.
                    XmlNode root = uml.SelectSingleNode("/modelRoot/types");
                    String xmlChild = "<modelClass name=\"" + relationName + "\">";
                    if (attrNames.Count != 0)
                    {
                        xmlChild += "<attributes>";
                        for (int i = 0; i < attrNames.Count; i++)
                            xmlChild += "<modelAttribute name=\"" + attrNames[i] + "\" type=\"" + attrTypes[i] + "\" />";
                        xmlChild += "</attributes>";
                    }
                    xmlChild += "<bidirectionalTargets>";
                    for (int i = 0; i < entitySets.Count; i++)
                    {
                        String sourceMultiplicity = "ZeroMany";
                        String targetMultiplicity = "One";
                        //if (cards[i] == "1") targetMultiplicity = "One";
                        xmlChild += "<bidirectionalAssociation sourceMultiplicity=\"" + sourceMultiplicity + "\" targetMultiplicity=\"" + targetMultiplicity + "\" >";
                        xmlChild += "<modelClassMoniker name=\"//" + entitySets[i] + "\"/>";
                        xmlChild += "</bidirectionalAssociation>";
                    }
                    xmlChild += "</bidirectionalTargets>";
                    xmlChild += "</modelClass>";
                    AddChild(xmlChild, root);
                }
            }
            uml.Save(umlFile);
        }

        private void AddChild(String xml, XmlNode node)
        {
            XmlDocument temp = new XmlDocument();
            temp.LoadXml(xml);
            XmlNode child = node.OwnerDocument.ImportNode(temp.DocumentElement, true);
            node.AppendChild(child);
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsForm f = new OptionsForm();
            if (f.ShowDialog() == DialogResult.OK)
            {
                isAttr = f.isAttr;
                isClass = f.isClass;
            }
        }

        private const String umlString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
        private const String xsltTable1 = "TranslationOne.xsl";
        private const String xsltTable2 = "TranslationTwo.xsl";
        private Boolean isAttr = true;
        private Boolean isClass = false;
    }
}
