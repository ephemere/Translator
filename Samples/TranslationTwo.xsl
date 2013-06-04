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

                <!-- Транслируем  отображения -->
                <!-- Перебираем все множества сущностей, участвующие где-либо -->
                <xsl:for-each select="elements/entitySet[count(role) != 0]">
                    <xsl:variable name="entitySetName" select="@name"/>
                    <modelClass name="{$entitySetName}">
                        <attributes>
                            <!-- Теперь нужно получить список атрибутов. Для этого переберем все роли и найдем соответствующие им множества значений -->
                            <xsl:for-each select="role/classifierReferencesRole/roleMoniker">
                                <xsl:variable name="preImageRoleName" select="@name"/>
                                <!-- Нашли роль прообраза. Находим аттрибутивное отображение с этой ролью. -->
                                <xsl:for-each select="/ermModel/attributeMapping/ermModelHasAttributeMapping/attributeMapping/inRole/mappingInRole/roleMoniker[@name = $preImageRoleName]">
                                    <xsl:variable name="mappingName" select="../../../@name"/>
                                    <xsl:variable name="imageRoleName" select="../../../outRole/mappingOutRole/roleMoniker/@name"/>
                                    <xsl:variable name="minCardinality" select="../../../@minCardinality"/>
                                    <xsl:variable name="maxCardinality" select="../../../@maxCardinality"/>
                                    <!-- Нашли роль образа. Теперь ищем множество значений. -->
                                    <xsl:for-each select="/ermModel/valueSet/ermModelHasValueSet/valueSet/role/classifierReferencesRole/roleMoniker[@name = $imageRoleName]">
                                        <xsl:variable name="valueSetName" select="../../../@name"/>
                                        <!-- Выводим аттрибут класса -->
                                        <xsl:choose>
                                            <xsl:when test="$minCardinality='0' and $maxCardinality='M'">
                                                <modelAttribute name="{$mappingName}" type="list" multiplicity="M"/>
                                            </xsl:when>
                                            <xsl:otherwise>
                                                <modelAttribute name="{$entitySetName}" type="{$valueSetName}"/>
                                            </xsl:otherwise>
                                        </xsl:choose>
                                    </xsl:for-each>
                                </xsl:for-each>
                            </xsl:for-each>
                        </attributes>
                        <!-- Разобрались с атрибутами. Теперь связи -->

                    </modelClass>
                </xsl:for-each>


            </types>
        </modelRoot>
    </xsl:template>
</xsl:stylesheet>
