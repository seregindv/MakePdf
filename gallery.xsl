<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <xsl:key name="p" match="/Gallery/Items/Item" use="concat(@Width, '-', @Height)"/>

  <xsl:template match="/">
    <root xmlns="http://www.w3.org/1999/XSL/Format">
      <!-- pages -->
      <layout-master-set>
        <simple-page-master master-name="p0" page-width="60mm" page-height="84mm">
          <region-body />
        </simple-page-master>
        <xsl:for-each select="Gallery/Items/Item[generate-id(.)=generate-id(key('p',concat(@Width, '-', @Height)))]">
          <xsl:element name="simple-page-master">
            <xsl:attribute name="master-name">
              <xsl:value-of select ="concat('p',@Width,'-',@Height)"/>
            </xsl:attribute>
            <xsl:attribute name="page-width">
              <xsl:value-of select="concat(@Width div 300,'in')"/>
            </xsl:attribute>
            <xsl:attribute name="page-height">
              <xsl:value-of select="concat(@Height div 300,'in')"/>
            </xsl:attribute>
            <region-body />
          </xsl:element>
        </xsl:for-each>
      </layout-master-set>
      <!-- cover page -->
      <!-- picture -->
      <!-- description -->
      <!--<xsl:for-each select="/Gallery/Items/Item">
      key for <xsl:value-of select="."/>=
      <xsl:value-of select="concat('p', @Width, '-', @Height)"/>
    </xsl:for-each>-->
    </root>
  </xsl:template>
  
</xsl:stylesheet>
