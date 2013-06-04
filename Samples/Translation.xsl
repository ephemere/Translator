<?xml version="1.0" encoding="windows-1251"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:output method="xml" indent="yes" encoding="UTF-8"/>

    <xsl:template match="/ermModel">
        <modelRoot xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="77dbda01-d2fa-4a8e-ae10-d9ddf313474c" name="" xmlns="http://schemas.microsoft.com/dsltools/UMLCreator">
            <types>
                <!-- сначала выбираем множества значений, нигде не участвующие -->
                <xsl:for-each select="valueSet/ermModelHasValueSet/valueSet[count(role) = 0]">
                    <xsl:variable name="name" select="@name"/>
                    <modelClass name="{$name}"/>
                </xsl:for-each>

                <!-- затем выбираем множества сущностей, также нигде не участвующие -->
                <xsl:for-each select="elements/entitySet[count(role) = 0]">
                    <xsl:variable name="name" select="@name"/>
                    <modelClass name="{$name}"/>
                </xsl:for-each>

                <!-- перебираем все полные функциональные отображения -->
                <xsl:for-each select="attributeMapping/ermModelHasAttributeMapping/attributeMapping[@minCardinality='1' and @maxCardinality='1']">
                    <xsl:variable name="preImageName" select="inRole/mappingInRole/roleMoniker/@name"/>
                    <xsl:variable name="imageName" select="outRole/mappingOutRole/roleMoniker/@name"/>
                    <xsl:variable name="attributeName" select="@name"/>
                    <!-- Нашли роль прообраза. Теперь ищем соответствующее ей множество сущностей. -->
                    <xsl:for-each select="/ermModel/elements/entitySet/role/classifierReferencesRole/roleMoniker[@name=$preImageName]">
                        <xsl:variable name="entitySetName" select="../../../@name"/>
                        <!-- Нашли множество сущностей. Теперь ищем множество значений -->
                        <xsl:for-each select="/ermModel/valueSet/ermModelHasValueSet/valueSet/role/classifierReferencesRole/roleMoniker[@name=$imageName]">
                            <xsl:variable name="valueSetName" select="../../../@name"/>
                            <!-- И наконец выводим готовое описание класса -->
                            <modelClass name="{$entitySetName}">
                                <attributes>
                                    <modelAttribute name="{$attributeName}" type="{$valueSetName}"/>
                                </attributes>
                            </modelClass>
                        </xsl:for-each>
                    </xsl:for-each>
                </xsl:for-each>

                <!-- перебираем все частичные функциональные отображения -->
                <xsl:for-each select="attributeMapping/ermModelHasAttributeMapping/attributeMapping[@minCardinality='0' and @maxCardinality='1']">
                    <xsl:variable name="preImageName" select="inRole/mappingInRole/roleMoniker/@name"/>
                    <xsl:variable name="imageName" select="outRole/mappingOutRole/roleMoniker/@name"/>
                    <xsl:variable name="attributeName" select="@name"/>
                    <!-- Нашли роль прообраза. Теперь ищем соответствующее ей множество сущностей. -->
                    <xsl:for-each select="/ermModel/elements/entitySet/role/classifierReferencesRole/roleMoniker[@name=$preImageName]">
                        <xsl:variable name="entitySetName" select="../../../@name"/>
                        <!-- Нашли множество сущностей. Теперь ищем множество значений -->
                        <xsl:for-each select="/ermModel/valueSet/ermModelHasValueSet/valueSet/role/classifierReferencesRole/roleMoniker[@name=$imageName]">
                            <xsl:variable name="valueSetName" select="../../../@name"/>
                            <!-- И наконец выводим готовое описание класса -->
                            <modelClass name="{$entitySetName}">
                                <attributes>
                                    <modelAttribute name="{$attributeName}" type="{$valueSetName}"/>
                                </attributes>
                            </modelClass>
                        </xsl:for-each>
                    </xsl:for-each>
                </xsl:for-each>

                <!-- перебираем все нефункциональные отображения -->
                <xsl:for-each select="attributeMapping/ermModelHasAttributeMapping/attributeMapping[@minCardinality='0' and @maxCardinality='M']">
                    <xsl:variable name="preImageName" select="inRole/mappingInRole/roleMoniker/@name"/>
                    <xsl:variable name="imageName" select="outRole/mappingOutRole/roleMoniker/@name"/>
                    <xsl:variable name="attributeName" select="@name"/>
                    <!-- Нашли роль прообраза. Теперь ищем соответствующее ей множество сущностей. -->
                    <xsl:for-each select="/ermModel/elements/entitySet/role/classifierReferencesRole/roleMoniker[@name=$preImageName]">
                        <xsl:variable name="entitySetName" select="../../../@name"/>
                        <!-- Нашли множество сущностей. Теперь ищем множество значений -->
                        <xsl:for-each select="/ermModel/valueSet/ermModelHasValueSet/valueSet/role/classifierReferencesRole/roleMoniker[@name=$imageName]">
                            <xsl:variable name="valueSetName" select="../../../@name"/>
                            <!-- И наконец выводим готовое описание класса -->
                            <modelClass name="{$entitySetName}">
                                <attributes>
                                    <modelAttribute name="{$attributeName}" type="{$valueSetName}"/>
                                </attributes>
                            </modelClass>
                        </xsl:for-each>
                    </xsl:for-each>
                </xsl:for-each>


            </types>
        </modelRoot>
    </xsl:template>
</xsl:stylesheet>
