<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes"/>
<xsl:template match="*[name()='ermModel']">
	<modelRoot xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="77dbda01-d2fa-4a8e-ae10-d9ddf313474c" name="" xmlns="http://schemas.microsoft.com/dsltools/UMLCreator">
  	<xsl:apply-templates select="*[name()='valueSet']//*[@name]"/>
	</modelRoot>
		
</xsl:template>

<xsl:template match="*[name()='valueSet']//*[@name]">
	<types>
		<xsl:variable name="name" select="//*[name()='valueSet'][@name]/@name"/>
		<modelClass name="{$name}"/>
	</types>
</xsl:template>

</xsl:stylesheet>