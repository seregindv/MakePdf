<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns="http://www.w3.org/1999/XSL/Format"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <xsl:output method="xml" indent="yes"/>

  <xsl:key name="p" match="/Gallery/Gallery/Items/GalleryItem[Size]" use="concat(Size/Width, '-', Size/Height)"/>

  <xsl:variable name="PortraitPageHeight">
    <xsl:choose>
      <xsl:when test="/Gallery/ScreenHeight &lt;= 0">84</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="/Gallery/ScreenHeight" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>
  <xsl:variable name="PortraitPageWidth">
    <xsl:choose>
      <xsl:when test="/Gallery/ScreenWidth &lt;= 0">60</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="/Gallery/ScreenWidth" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>
  <xsl:variable name="LandscapePageWidth">
    <xsl:choose>
      <xsl:when test="/Gallery/ScreenWidth &lt;= 0">95</xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$PortraitPageHeight" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:variable>
  <xsl:variable name="LandscapePageHeight" select="$PortraitPageWidth" />
  <xsl:variable name="LeftMargin" select="2" />
  <xsl:variable name="RightMargin" select="2" />
  <xsl:variable name="ParagraphIndent" select="'4mm'" />

  <xsl:variable name="apos" select='"&apos;"'/>
  <xsl:variable name="IllustratedArticle">
    <xsl:choose>
      <xsl:when test="/Gallery/Gallery/@xsi:type = 'TextGallery'
                or /Gallery/Gallery/@xsi:type = 'MotorArticleGallery'
                or /Gallery/Gallery/@xsi:type = 'LentaInterview'
                or /Gallery/Gallery/@xsi:type = 'SlonAuthorGallery'
                or /Gallery/Gallery/@xsi:type = 'LentaBeelineGallery'
                or /Gallery/Gallery/@xsi:type = 'MedusaInterview'
                or /Gallery/Gallery/@xsi:type = 'RbcInterview'">True</xsl:when>
      <xsl:otherwise>False</xsl:otherwise>
    </xsl:choose>
  </xsl:variable>

  <xsl:template match="/">
    <root>
      <!-- pages -->
      <layout-master-set>
        <xsl:element name="simple-page-master">
          <xsl:attribute name="master-name">p0</xsl:attribute>
          <xsl:attribute name="page-width">
            <xsl:value-of select="concat($PortraitPageWidth, 'mm')" />
          </xsl:attribute>
          <xsl:attribute name="page-height">
            <xsl:value-of select="concat($PortraitPageHeight, 'mm')" />
          </xsl:attribute>
          <xsl:attribute name="margin-left">
            <xsl:value-of select="concat($LeftMargin, 'mm')" />
          </xsl:attribute>
          <xsl:attribute name="margin-right">
            <xsl:value-of select="concat($RightMargin, 'mm')" />
          </xsl:attribute>
          <region-body />
        </xsl:element>
        <xsl:element name="simple-page-master">
          <xsl:attribute name="master-name">p1</xsl:attribute>
          <xsl:attribute name="page-width">
            <xsl:value-of select="concat($LandscapePageWidth, 'mm')" />
          </xsl:attribute>
          <xsl:attribute name="page-height">
            <xsl:value-of select="concat($LandscapePageHeight, 'mm')" />
          </xsl:attribute>
          <xsl:attribute name="margin-left">
            <xsl:value-of select="concat($LeftMargin, 'mm')" />
          </xsl:attribute>
          <xsl:attribute name="margin-right">
            <xsl:value-of select="concat($RightMargin, 'mm')" />
          </xsl:attribute>
          <region-body />
        </xsl:element>
        <xsl:for-each select="/Gallery/Gallery/Items/GalleryItem[Size and generate-id(.)=generate-id(key('p',concat(Size/Width, '-', Size/Height)))]">
          <xsl:element name="simple-page-master">
            <xsl:attribute name="master-name">
              <xsl:value-of select ="concat('p',Size/Width,'-',Size/Height)"/>
            </xsl:attribute>
            <xsl:attribute name="page-width">
              <xsl:value-of select="concat(Size/Width div 300,'in')"/>
            </xsl:attribute>
            <xsl:attribute name="page-height">
              <xsl:value-of select="concat(Size/Height div 300,'in')"/>
            </xsl:attribute>
            <region-body />
          </xsl:element>
        </xsl:for-each>
      </layout-master-set>
      <!-- cover page -->
      <page-sequence master-reference="p0">
        <flow flow-name="xsl-region-body" font-family="Tahoma" text-align="justify">
          <xsl:element name="table">
            <xsl:attribute name="width">
              <xsl:value-of select="concat($PortraitPageWidth - $LeftMargin - $RightMargin, 'mm')" />
            </xsl:attribute>
            <xsl:attribute name="height">
              <xsl:value-of select="concat($PortraitPageHeight, 'mm')"/>
            </xsl:attribute>
            <xsl:attribute name="table-layout">fixed</xsl:attribute>
            <table-column />
            <table-body>
              <table-row height="1mm">
                <table-cell text-align="right" padding-top="1mm">
                  <block font-size="3pt">
                    <xsl:value-of select="/Gallery/Date" />
                  </block>
                </table-cell>
              </table-row>
              <xsl:element name="table-row">
                <xsl:attribute name="height">
                  <xsl:value-of select="concat($PortraitPageHeight - 1, 'mm')"/>
                </xsl:attribute>
                <table-cell text-align="center" display-align="center">
                  <block font-size="20pt">
                    <xsl:value-of select ="/Gallery/Name" />
                  </block>
                  <block font-size="14pt">
                    <xsl:value-of select ="/Gallery/Annotation" />
                  </block>
                </table-cell>
              </xsl:element>
            </table-body>
          </xsl:element>
          <!-- contents -->
          <xsl:call-template name="ProcessTags">
            <xsl:with-param name="Tags" select="/Gallery/Tags" />
          </xsl:call-template>
          <xsl:if test="not(/Gallery/Gallery/Items/GalleryItem)">
            <xsl:call-template name="SourceParagraph" />
          </xsl:if>
        </flow>
      </page-sequence>
      <!-- gallery -->
      <xsl:for-each select="/Gallery/Gallery/Items/GalleryItem">
        <!--picture-->
        <xsl:if test="Size">
          <xsl:element name="page-sequence">
            <xsl:attribute name="master-reference">
              <xsl:value-of select="concat('p',Size/Width,'-',Size/Height)" />
            </xsl:attribute>
            <flow flow-name="xsl-region-body">
              <xsl:element name="external-graphic">
                <xsl:attribute name="src">
                  <xsl:value-of select="concat('url(',$apos,LocalPath,$apos,')')" />
                </xsl:attribute>
                <xsl:attribute name="content-height">
                  <xsl:value-of select="concat(Size/Height div 300,'in')" />
                </xsl:attribute>
                <xsl:attribute name="content-width">
                  <xsl:value-of select="concat(Size/Width div 300,'in')" />
                </xsl:attribute>
              </xsl:element>
            </flow>
          </xsl:element>
        </xsl:if>
        <!--description-->
        <xsl:if test="Tags/Tag or (last() = position() and /Gallery/SourceAddress)">
          <xsl:element name="page-sequence">
            <xsl:attribute name="master-reference">
              <xsl:choose>
                <xsl:when test="$IllustratedArticle = 'True' or Size/Width &lt; Size/Height">p0</xsl:when>
                <xsl:otherwise>p1</xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
            <flow flow-name="xsl-region-body" font-family="Tahoma" text-align="justify">
              <xsl:call-template name="ProcessTags">
                <xsl:with-param name="Tags" select="./Tags" />
              </xsl:call-template>
              <xsl:if test="last() = position()">
                <xsl:call-template name="SourceParagraph" />
              </xsl:if>
            </flow>
          </xsl:element>
        </xsl:if>
      </xsl:for-each>
    </root>
  </xsl:template>

  <xsl:template name="Color">
    <xsl:if test="./@Color">
      <xsl:attribute name="color">
        <xsl:value-of select="./@Color" />
      </xsl:attribute>
    </xsl:if>
  </xsl:template>

  <xsl:template name="ProcessTags">
    <xsl:param name="Tags" />
    <xsl:for-each select="$Tags/Tag">
      <xsl:choose>
        <xsl:when test="./@xsi:type = 'Paragraph'">
          <xsl:element name="block">
            <xsl:attribute name="text-indent">
              <xsl:value-of select="$ParagraphIndent"/>
            </xsl:attribute>
            <xsl:call-template name="Color" />
            <xsl:call-template name="ProcessTags">
              <xsl:with-param name="Tags" select="./Tags" />
            </xsl:call-template>
          </xsl:element>
        </xsl:when>
        <xsl:when test="./@xsi:type = 'Bold'">
          <inline font-weight="bold">
            <xsl:call-template name="ProcessTags">
              <xsl:with-param name="Tags" select="./Tags" />
            </xsl:call-template>
          </inline>
        </xsl:when>
        <xsl:when test="./@xsi:type = 'Italic'">
          <inline font-style="italic">
            <xsl:call-template name="ProcessTags">
              <xsl:with-param name="Tags" select="./Tags" />
            </xsl:call-template>
          </inline>
        </xsl:when>
        <xsl:when test="./@xsi:type = 'Text'">
          <xsl:for-each select="text()">
            <xsl:value-of select="." />
          </xsl:for-each>
          <xsl:if test="./Tags/Tag">
            <xsl:call-template name="ProcessTags">
              <xsl:with-param name="Tags" select="./Tags" />
            </xsl:call-template>
          </xsl:if>
        </xsl:when>
        <xsl:when test="./@xsi:type = 'Ref'">
          <xsl:element name="basic-link">
            <xsl:attribute name="text-decoration">underline</xsl:attribute>
            <xsl:attribute name="color">blue</xsl:attribute>
            <xsl:attribute name="external-destination">
              <xsl:value-of select="./@Address" />
            </xsl:attribute>
            <xsl:call-template name="ProcessTags">
              <xsl:with-param name="Tags" select="./Tags" />
            </xsl:call-template>
          </xsl:element>
        </xsl:when>
        <xsl:when test="./@xsi:type = 'Color'">
          <xsl:element name="inline">
            <xsl:attribute name="color">
              <xsl:value-of select="./@Value" />
            </xsl:attribute>
            <xsl:call-template name="ProcessTags">
              <xsl:with-param name="Tags" select="./Tags" />
            </xsl:call-template>
          </xsl:element>
        </xsl:when>
        <!-- unknown tag -->
        <xsl:otherwise>
          <xsl:value-of select="." />
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="SourceParagraph">
    <xsl:if test="/Gallery/SourceAddress">
      <block text-indent="4mm">
        Источник: <xsl:element name="basic-link">
          <xsl:attribute name="text-decoration">underline</xsl:attribute>
          <xsl:attribute name="color">blue</xsl:attribute>
          <xsl:attribute name="external-destination">
            <xsl:value-of select="/Gallery/SourceAddress" />
          </xsl:attribute>
          <xsl:value-of select="/Gallery/SourceName" />
        </xsl:element>
      </block>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>
