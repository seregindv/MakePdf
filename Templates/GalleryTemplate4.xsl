<!-- Lines, no Tags -->
<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <xsl:key name="p" match="/Gallery/Gallery/Items/GalleryItem" use="concat(Size/Width, '-', Size/Height)"/>

  <xsl:variable name="apos" select='"&apos;"'/>
  <xsl:template match="/">
    <root xmlns="http://www.w3.org/1999/XSL/Format">
      <!-- pages -->
      <layout-master-set>
        <simple-page-master master-name="p0" page-width="60mm" page-height="84mm" margin-left="2mm" margin-right="2mm">
          <region-body />
        </simple-page-master>
        <simple-page-master master-name="p1" page-width="95mm" page-height="60mm" margin-left="2mm" margin-right="2mm">
          <region-body />
        </simple-page-master>
        <xsl:for-each select="/Gallery/Gallery/Items/GalleryItem[generate-id(.)=generate-id(key('p',concat(Size/Width, '-', Size/Height)))]">
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
          <table width="56mm" height="84mm" table-layout="fixed">
            <table-column />
            <table-body>
              <table-row height="25mm">
                <table-cell>
                  <block />
                </table-cell>
              </table-row>
              <table-row height="59mm">
                <table-cell text-align="center" vertical-align="middle">
                  <block font-size="20pt">
                    <xsl:value-of select ="/Gallery/Name" />
                  </block>
                  <block font-size="14pt">
                    <xsl:value-of select ="/Gallery/Annotation" />
                  </block>
                </table-cell>
              </table-row>
            </table-body>
          </table>
          <!-- contents -->
          <xsl:call-template name="TextLines">
            <xsl:with-param name="LinesRoot" select="/Gallery" />
          </xsl:call-template>
        </flow>
      </page-sequence>
      <!-- gallery -->
      <xsl:for-each select="/Gallery/Gallery/Items/GalleryItem">
        <!--picture-->
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
        <!--description-->
        <xsl:if test="Lines/Line or last() = position()">
          <xsl:element name="page-sequence">
            <xsl:attribute name="master-reference">
              <xsl:choose>
                <xsl:when test="Size/Width &lt; Size/Height">p0</xsl:when>
                <xsl:otherwise>p1</xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
            <flow flow-name="xsl-region-body" font-family="Tahoma" text-align="justify">
              <xsl:call-template name="TextLines">
                <xsl:with-param name="LinesRoot" select="." />
              </xsl:call-template>
              <xsl:if test="last() = position()">
                <block text-indent="4mm">
                  Источник: <xsl:element name="basic-link">
                    <xsl:attribute name="text-decoration">underline</xsl:attribute>
                    <xsl:attribute name="color">blue</xsl:attribute>
                    <xsl:attribute name="external-destination">
                      <xsl:value-of select="/Gallery/SourceAddress" />
                    </xsl:attribute>
                    <xsl:value-of select="/Gallery/SourceAddress" />
                  </xsl:element>
                </block>
              </xsl:if>
            </flow>
          </xsl:element>
        </xsl:if>
      </xsl:for-each>
    </root>
  </xsl:template>

  <xsl:template name="TextLines">
    <xsl:param name="LinesRoot" />
    <xsl:for-each select="$LinesRoot/Lines/Line">
      <block text-indent="4mm" xmlns="http://www.w3.org/1999/XSL/Format">
        <xsl:value-of select="." />
      </block>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>
