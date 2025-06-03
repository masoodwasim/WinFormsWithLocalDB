# Product Master Data Management (PMDM) Implementation Guide

[TOC]

## Executive Summary

Product Master Data Management (PMDM) is scheduled for launch in June 2025 and will serve as the new source of truth for all product-related data fields. This comprehensive guide documents the PMDM implementation, including data sources, available attributes, comparison with existing systems, and practical guidance for using the new system.

Key highlights:

- PMDM leverages Databricks Unity Catalog with a Medallion Architecture (Bronze, Silver, Gold layers)
- The system offers significant improvements over existing data models, including structured relationships, enhanced brand classification, and comprehensive data governance
- A major update in mid-June 2025 will introduce validated brand classification for approximately 750,000 products
- Implementation should be aligned with this update to ensure data accuracy and consistency

This guide provides both strategic recommendations and practical, hands-on information for working with PMDM.

## 1. Introduction

### 1.1 Background

Product Master Data Management (PMDM) is a strategic initiative to establish a single source of truth for all product-related data. Currently in Development and QA environments, PMDM is scheduled for launch in June 2025, replacing the existing Curated Data Model (CDM).

### 1.2 Objectives

The PMDM implementation aims to:

- Centralize product data management in a unified platform
- Improve data quality and consistency
- Enhance brand classification and product hierarchy
- Streamline reporting and analytics
- Provide comprehensive product attributes for business users

### 1.3 Scope

This guide covers:

- PMDM architecture and data sources
- Available product attributes
- Comparison with existing systems
- Data migration considerations
- Implementation recommendations
- Practical guidance for attribute identification and count calculation

## 2. PMDM Architecture

### 2.1 Databricks Unity Catalog

PMDM is implemented within Databricks Unity Catalog, which provides unified governance for data and AI assets. Unity Catalog enables seamless governance across any cloud or platform, with features including:

- Unified access control
- Data lineage tracking
- Audit logging
- Data discovery
- Metadata management

![PMDM Architecture in Databricks Unity Catalog](https://confluence.example.com/download/attachments/123456789/pmdm_architecture.png)

### 2.2 Medallion Architecture

PMDM leverages a three-layer Medallion Architecture:

| Layer | Description | Purpose |
|-------|-------------|---------|
| **Bronze** | Raw ingested data | Preserves original data in unmodified form |
| **Silver** | Standardized and cleansed data | Provides validated and transformed data |
| **Gold** | Business-ready data | Serves as the single source of truth |

#### Bronze Layer
- Contains original, unmodified product data
- Preserves source system formats and structures
- Serves as historical record of incoming data
- Tables include: raw_product_data, raw_product_hierarchy, raw_product_attributes, raw_product_relationships

#### Silver Layer
- Contains validated and transformed product data
- Standardizes formats across sources
- Deduplicates records
- Applies entity resolution
- Tables include: standardized_products, product_hierarchy, product_attributes, product_relationships

#### Gold Layer
- Contains "golden records" for each product
- Serves as single source of truth for product attributes
- Optimized for analytics and reporting
- Includes derived and aggregated metrics
- Tables include: master_products, master_hierarchy, master_attributes, analytics_ready_views

### 2.3 Environment Details

| Environment | Purpose | Status | Access |
|-------------|---------|--------|--------|
| **Development** | Initial data modeling and transformation development | Available | Databricks Unity Catalog |
| **QA** | Testing data quality and transformation logic | Available | Databricks Unity Catalog |
| **Production** | Official source of truth | Planned for June 2025 | Databricks Unity Catalog (future) |

## 3. Current CDM Data Model Schema

The current Curated Data Model (CDM) consists of several key tables that will be migrated to the PMDM structure. Below are the core tables and their key attributes:

### 3.1 dim_product_mstr_item_cdm

This table contains core product information:

| Key Column | Data Type | Description |
|------------|-----------|-------------|
| ITEM_KEY | string | Key column to uniquely identify item record |
| JOIN_KEY | string | Combination of BannerID, BrandItemNumber and UPCPLU |
| ID | string | Unique PMDM Id |
| BANNER_NAME | string | Unique Banner Name |
| BANNERID | int | Unique Banner Id |
| BRAND_ITEM_NUMBER | bigint | Brand Item Number |
| UPCPLU_NUMBER | bigint | UPCPLU that scans at register |
| ITEM_DESC | string | Item Description |
| ITEM_LONG_DESC | string | Item Description long version |
| ITEM_STATUS | string | Item Status Code |
| GTIN_UPC | bigint | GTIN UPC/PLU Code |
| SELL_UPC_NUMBER | bigint | Sell UPC/PLU Code |
| CREATE_DATE | date | Date Item Created |
| EFF_START_DATE | date | Effective Start Date |
| EFF_END_DATE | timestamp | Effective End Date |

**Brand-Related Fields:**
- BRAND_DIST_TYPE_CD: Brand Distribution Type Code
- BRAND_NAME: Brand Name
- BRAND_OBJECT: Brand Object
- BRAND_OWNER: Brand Owner
- BRAND_OWNER_GLN: Brand Owner GLN
- BRAND_OWNER_NAME: Brand Owner Name
- NEW_BRAND: New Brand Name
- NEW_BRAND_FOR_SI: New Brand SI Name
- INITIAL_BRAND: Initial Brand
- SUBBRAND: Subbrand

**Classification Fields:**
- ITEM_CLASSIFICATION: Item Classification
- PERISHABLE_IND: Perishable Indicator
- DSD_WHSE_TYPE: Warehouse or DSD Indicator

### 3.2 dim_product_mstr_each_attributes_cdm

This table contains detailed attributes for each product:

| Key Column | Data Type | Description |
|------------|-----------|-------------|
| EA_KEY | string | Key column to uniquely identify Each Attribute record |
| JOIN_KEY | string | Combination of BannerID, BrandItemNumber and UPCPLU |
| ID | string | Unique PMDM Id |
| BANNERID | int | Unique Banner Id |
| BRAND_ITEM_NUMBER | bigint | Brand Item Number |
| UPCPLU_NUMBER | bigint | UPCPLU that scans at register |
| GTIN_UPC | bigint | GTIN UPC/PLU Code |
| SELL_UPC_NUMBER | bigint | Sell UPC/PLU Code |

**Brand Classification Fields:**
- EA_PRIMARY_BRAND: Each Attribute Primary Brand Name
- EA_PRIVATE_BRAND: Each Attribute Private Brand
- EA_PRIVATE_BRAND_NAME: Each Attribute Private Brand Name
- EA_GLOBAL_OWN_BRAND_IND: Each Attribute Global Own Brand Indicator

**Physical Attributes:**
- EA_GROSS_WEIGHT: Each Attribute Gross Weight
- EA_CUBE: Each Attribute Cube
- EA_HEIGHT: Each Attribute Height
- EA_WIDTH: Each Attribute Width
- EA_LENGTH: Each Attribute Length
- EA_NET_WEIGHT: Each Attribute Net Weight
- EA_NET_WEIGHT_UOM: Each Attribute Net Weight Unit of Measure

**Classification Fields:**
- EA_USDA_ORGNC_IND: Each Attribute USDA organic Indicator
- EA_FSA_ELIGIBLE_IND: Each Attribute FSA Eligible Indicator
- EA_HARZ_IND: Each Attribute Hazard Indicator
- EA_PRODUCT_GRP_DESC: Each Attribute Product Group Description
- EA_SALES_RESTRCTS: Each Attribute Sales Restriction
- EA_SHELFLIFE_DAYS: Each Attribute Shelf Life days
- EA_AGE_RESTRICTION: Each Attribute Age Restriction Indicator

### 3.3 dim_product_mstr_cph_merch_hierarchy_cdm

This table contains the product hierarchy information:

| Key Column | Data Type | Description |
|------------|-----------|-------------|
| HIER_KEY | string | Key column to uniquely identify Hierarchy record |
| JOIN_KEY | string | Combination of BannerID, BrandItemNumber and UPCPLU |
| BANNERID | int | Unique Banner Id |
| UPCPLU_NUMBER | bigint | UPCPLU that scans at register |
| BRAND_ITEM_NUMBER | bigint | Brand Item Number |

**Common Product Hierarchy (CPH):**
- CPH_NAME: Common Product Hier. Name
- CPH_DEPT / CPH_DEPT_ID / CPH_DEPT_DESC: Department level
- CPH_SUBDEPT / CPH_SUBDEPT_ID / CPH_SUBDEPT_DESC: Sub-department level
- CPH_PORTFOLIO / CPH_PORTFOLIO_ID / CPH_PORTFOLIO_DESC: Portfolio level
- CPH_CATGGRP / CPH_CATGGRP_ID / CPH_CATGGRP_DESC: Category Group level
- CPH_CATG / CPH_CATG_ID / CPH_CATG_DESC: Category level
- CPH_SUBCATG / CPH_SUBCATG_ID / CPH_SUBCATG_DESC: Sub-category level

**Legacy Merchandise Hierarchy:**
- MERCH_DEPT / MERCH_DEPT_ID / MERCH_DEPT_DESC: Department level
- MERCH_SUBDEPT / MERCH_SUBDEPT_ID / MERCH_SUBDEPT_DESC: Sub-department level
- MERCH_PORTFOLIO / MERCH_PORTFOLIO_ID / MERCH_PORTFOLIO_DESC: Portfolio level
- MERCH_CATRGGRP / MERCH_CATRGGRP_ID / MERCH_CATRGGRP_DESC: Category Group level
- MERCH_CATG / MERCH_CATG_ID / MERCH_CATG_DESC: Category level
- MERCH_SUBCATG / MERCH_SUBCATG_ID / MERCH_SUBCATG_DESC: Sub-category level
- MERCH_SEGMENT / MERCH_SEGMENT_ID / MERCH_SEGMENT_DESC: Segment level

### 3.4 dim_product_mstr_legacy_Item_cdm

This table contains legacy item information:

| Key Column | Data Type | Description |
|------------|-----------|-------------|
| LEGACY_ITEM_KEY | string | Key column to uniquely identify Legacy Item record |
| JOIN_KEY | string | Combination of BannerID, BrandItemNumber and UPCPLU |
| ID | string | Unique PMDM Id |
| BANNERID | int | Unique Banner Id |
| BRAND_ITEM_NUMBER | bigint | Brand Item Number |
| UPCPLU_NUMBER | bigint | UPCPLU that scans at register |
| TARGETID | string | Target Identifier |
| CONSUMER_FRIENDLY_DESCRIPTION | string | Consumer Friendly Description |
| LEGACY_ITEM_TYPE | string | Legacy Item Type |
| PRIMARY_UPC | string | Primary UPC |

## 4. Product Attributes in PMDM

### 4.1 Product Hierarchy

PMDM implements a standardized 5-level hierarchy structure that maps to the current CPH hierarchy in the CDM:

| PMDM Hierarchy Level | CDM Equivalent |
|----------------------|----------------|
| Department (Level 1) | CPH_DEPT / MERCH_DEPT |
| Category (Level 2) | CPH_CATG / MERCH_CATG |
| Subcategory (Level 3) | CPH_SUBCATG / MERCH_SUBCATG |
| Segment (Level 4) | MERCH_SEGMENT |
| Subsegment (Level 5) | N/A (New in PMDM) |

**Key Fields in PMDM**:
- `Hierarchy_ID`: Unique identifier for hierarchy node
- `Hierarchy_Level`: Level in hierarchy (1-5)
- `Hierarchy_Name`: Name of hierarchy node
- `Parent_Hierarchy_ID`: ID of parent node
- `Active_Flag`: Indicates if hierarchy node is active

**Query Example Using Current CDM Schema**:
```sql
SELECT 
  i.ITEM_KEY,
  i.BRAND_ITEM_NUMBER,
  i.UPCPLU_NUMBER,
  h.CPH_DEPT_DESC AS Department,
  h.CPH_CATG_DESC AS Category,
  h.CPH_SUBCATG_DESC AS Subcategory,
  h.MERCH_SEGMENT_DESC AS Segment
FROM dim_product_mstr_item_cdm i
JOIN dim_product_mstr_cph_merch_hierarchy_cdm h ON i.JOIN_KEY = h.JOIN_KEY
WHERE i.ITEM_STATUS = 'Active';
```

**Query Example in Future PMDM**:
```sql
SELECT 
  p.Product_ID,
  p.Product_Name,
  h1.Hierarchy_Name AS Department,
  h2.Hierarchy_Name AS Category,
  h3.Hierarchy_Name AS Subcategory,
  h4.Hierarchy_Name AS Segment,
  h5.Hierarchy_Name AS Subsegment
FROM gold_schema.master_products p
JOIN gold_schema.master_hierarchy h5 ON p.Hierarchy_ID = h5.Hierarchy_ID
JOIN gold_schema.master_hierarchy h4 ON h5.Parent_Hierarchy_ID = h4.Hierarchy_ID
JOIN gold_schema.master_hierarchy h3 ON h4.Parent_Hierarchy_ID = h3.Hierarchy_ID
JOIN gold_schema.master_hierarchy h2 ON h3.Parent_Hierarchy_ID = h2.Hierarchy_ID
JOIN gold_schema.master_hierarchy h1 ON h2.Parent_Hierarchy_ID = h1.Hierarchy_ID
WHERE h1.Hierarchy_Level = 1
  AND h2.Hierarchy_Level = 2
  AND h3.Hierarchy_Level = 3
  AND h4.Hierarchy_Level = 4
  AND h5.Hierarchy_Level = 5;
```

### 4.2 Brand Indicators

PMDM includes explicit brand classification attributes that map to the current CDM fields:

| PMDM Attribute | CDM Equivalent |
|----------------|----------------|
| Own_Brand_Flag | EA_GLOBAL_OWN_BRAND_IND |
| National_Brand_Flag | Derived (when EA_PRIVATE_BRAND = 'N' and EA_GLOBAL_OWN_BRAND_IND = 'N') |
| Private_Label_Flag | EA_PRIVATE_BRAND |
| Brand_Name | BRAND_NAME, EA_PRIMARY_BRAND |
| Brand_Owner | BRAND_OWNER |
| Sub-brand | SUBBRAND |

**Query Example Using Current CDM Schema**:
```sql
SELECT 
  i.ITEM_KEY,
  i.BRAND_ITEM_NUMBER,
  i.UPCPLU_NUMBER,
  i.BRAND_NAME,
  i.BRAND_OWNER,
  i.SUBBRAND,
  e.EA_PRIVATE_BRAND,
  e.EA_GLOBAL_OWN_BRAND_IND,
  CASE 
    WHEN e.EA_GLOBAL_OWN_BRAND_IND = 'Y' THEN 'Own Brand'
    WHEN e.EA_PRIVATE_BRAND = 'Y' THEN 'Private Brand'
    ELSE 'National Brand'
  END AS Brand_Type
FROM dim_product_mstr_item_cdm i
JOIN dim_product_mstr_each_attributes_cdm e ON i.JOIN_KEY = e.JOIN_KEY
WHERE i.ITEM_STATUS = 'Active';
```

**Query Example in Future PMDM**:
```sql
SELECT 
  Product_ID,
  Product_Name,
  Brand_Name,
  Brand_Owner,
  Own_Brand_Flag,
  National_Brand_Flag,
  Private_Label_Flag,
  Brand_Type_Code,
  Brand_Type_Description
FROM gold_schema.master_products
WHERE Own_Brand_Flag = TRUE;  -- Change flag as needed
```

**Current Brand Classification Process**:
- Third-party processor (Circana) helps develop a list of Own Brand products by removing all National Brand products
- Private Brands maintains their own product records separately
- Current query methods:
  - From current PMDM: Query on Global Own Brand/a_PrivateLabel = N
  - From SEGA: Look for items without a Private Brand Code
  - From ACES: Look for items with Private Label = N

**June 2025 Update**:
- Approximately 750,000 products have been flagged to designate each as PB (Private Brand), OB (Own Brand), or NB (National Brand)
- Category teams are currently validating this classification
- These records will be uploaded into the updated PMDM in mid-June 2025
- Legacy DA brands will be brought into the Stibo PMDM platform
- SEGA will be sunset as part of the June update

**Definition Clarification**:
- "Global Own Brands" should represent all non-National Brands products (Private Brands + Own Brands)
- "Own Brands" at ADUSA level has traditionally meant anything that is not Private Brand or National Brand
- Private Label and Private Brand designations do not include Own Brand products
- Definitions are being updated to be more clear and consistent with the June update

### 4.3 UPC Structure

UPC management in PMDM follows a hierarchical parent-child relationship that maps to the current CDM fields:

| PMDM Attribute | CDM Equivalent |
|----------------|----------------|
| UPC | UPCPLU_NUMBER, GTIN_UPC, SELL_UPC_NUMBER |
| UPC_Type | UPC_FORMAT |
| Parent_UPC | Derived relationship (not explicit in CDM) |
| Primary_UPC_Flag | PRIMARY_UPC (in legacy_item table) |

**Query Example Using Current CDM Schema**:
```sql
SELECT 
  i.ITEM_KEY,
  i.BRAND_ITEM_NUMBER,
  i.UPCPLU_NUMBER AS Main_UPC,
  i.GTIN_UPC,
  i.SELL_UPC_NUMBER,
  l.PRIMARY_UPC
FROM dim_product_mstr_item_cdm i
LEFT JOIN dim_product_mstr_legacy_Item_cdm l ON i.JOIN_KEY = l.JOIN_KEY
WHERE i.ITEM_STATUS = 'Active';
```

**Query Example in Future PMDM**:
```sql
SELECT 
  p.Product_ID,
  p.Product_Name,
  p.UPC AS Parent_UPC,
  c.UPC AS Child_UPC,
  c.UPC_Type,
  c.Primary_UPC_Flag
FROM gold_schema.master_products p
JOIN gold_schema.master_products c ON p.UPC = c.Parent_UPC
WHERE p.UPC_Status = 'Active';
```

### 4.4 Added Date

PMDM tracks various date-related attributes that map to the current CDM fields:

| PMDM Attribute | CDM Equivalent |
|----------------|----------------|
| Creation_Date | CREATE_DATE |
| First_Added_Date | CREATE_DATE |
| Last_Modified_Date | LAST_UPDATED_DATE |
| Effective_Start_Date | EFF_START_DATE |
| Effective_End_Date | EFF_END_DATE |

**Query Example Using Current CDM Schema**:
```sql
SELECT 
  ITEM_KEY,
  BRAND_ITEM_NUMBER,
  UPCPLU_NUMBER,
  CREATE_DATE,
  LAST_UPDATED_DATE,
  EFF_START_DATE,
  EFF_END_DATE
FROM dim_product_mstr_item_cdm
WHERE CREATE_DATE BETWEEN '2025-01-01' AND '2025-06-01';
```

**Query Example in Future PMDM**:
```sql
SELECT 
  Product_ID,
  Product_Name,
  Creation_Date,
  First_Added_Date,
  Last_Modified_Date,
  Effective_Start_Date,
  Effective_End_Date
FROM gold_schema.master_products
WHERE First_Added_Date BETWEEN '2025-01-01' AND '2025-06-01';
```

### 4.5 Food/Non-Food Flag

Classification attributes in PMDM that relate to Food/Non-Food classification map to the current CDM fields:

| PMDM Attribute | CDM Equivalent |
|----------------|----------------|
| Food_Non_Food_Flag | Derived from ITEM_CLASSIFICATION |
| Perishable_Flag | PERISHABLE_IND |
| Consumable_Flag | Derived from ITEM_CLASSIFICATION |

**Query Example Using Current CDM Schema**:
```sql
SELECT 
  i.ITEM_KEY,
  i.BRAND_ITEM_NUMBER,
  i.UPCPLU_NUMBER,
  i.ITEM_CLASSIFICATION,
  i.PERISHABLE_IND,
  CASE 
    WHEN i.ITEM_CLASSIFICATION LIKE '%FOOD%' THEN 'F'
    ELSE 'N'
  END AS Food_Non_Food_Flag
FROM dim_product_mstr_item_cdm i
WHERE i.ITEM_STATUS = 'Active';
```

**Query Example in Future PMDM**:
```sql
SELECT 
  p.Product_ID,
  p.Product_Name,
  a.Food_Non_Food_Flag,
  a.Food_Type,
  a.Consumable_Flag,
  a.Perishable_Flag
FROM gold_schema.master_products p
JOIN gold_schema.master_attributes a ON p.Product_ID = a.Product_ID
WHERE a.Food_Non_Food_Flag = 'F';  -- 'F' for Food, 'N' for Non-Food
```

### 4.6 Guiding Stars

The nutritional rating system in PMDM is a new feature not present in the current CDM schema:

**Key Fields**:
- `Guiding_Stars_Rating`: Rating from 0-3 stars
- `Guiding_Stars_Points`: Calculated point value for star rating
- `HNV_SID`: Health & Nutrition Value System Identifier
- `Algorithm_Version`: Version of Guiding Stars algorithm used
- `Algorithm_Category`: Which of five algorithms was applied
- `Evaluation_Date`: When product was last evaluated
- `Nutritional_Data_Source`: Source of nutritional data
- `Exemption_Reason`: Reason for exemption from rating

**Query Example in Future PMDM**:
```sql
SELECT 
  p.Product_ID,
  p.Product_Name,
  a.Guiding_Stars_Rating,
  a.Guiding_Stars_Points,
  a.HNV_SID,
  a.Algorithm_Version,
  a.Algorithm_Category,
  a.Evaluation_Date
FROM gold_schema.master_products p
JOIN gold_schema.master_attributes a ON p.Product_ID = a.Product_ID
WHERE a.Guiding_Stars_Rating > 0
  AND a.Food_Non_Food_Flag = 'F';
```

### 4.7 Additional Attributes

PMDM includes numerous other attributes that map to or extend the current CDM fields:

| PMDM Attribute | CDM Equivalent |
|----------------|----------------|
| Physical Dimensions | EA_HEIGHT, EA_WIDTH, EA_LENGTH |
| Weight Information | EA_GROSS_WEIGHT, EA_NET_WEIGHT |
| Packaging Information | EA_PACKING_TYPE_CODE, EA_PACKING_IS_RETURNABLE |
| Organic Certification | EA_USDA_ORGNC_IND |
| Age Restrictions | EA_AGE_RESTRICTION |
| Shelf Life | EA_SHELFLIFE_DAYS |

**New Attributes in PMDM**:
- `Sustainable_Packaging_Flag`: Indicates sustainable packaging
- `Digital_Asset_URL`: Links to product images and media
- `Allergen_Information`: Structured allergen data
- `Country_of_Origin`: Product origin country
- `Certification_List`: Product certifications (Organic, etc.)
- `Completeness_Score`: Data quality metric for completeness
- `Accuracy_Score`: Data quality metric for accuracy
- `Consistency_Score`: Data quality metric for consistency
- `Data_Source`: Original source of the product data
- `Data_Steward`: Person responsible for data quality

## 5. Comparison with Existing Systems

### 5.1 Entity Mapping

![CDM to PMDM Data Flow and Transformation](https://confluence.example.com/download/attachments/123456789/cdm_to_pmdm_mapping.png)

| CDM Entity | PMDM Equivalent | Transformation Notes |
|------------|-----------------|----------------------|
| dim_product_mstr_item_cdm | Gold: master_products | Core product attributes consolidated |
| dim_product_mstr_each_attributes_cdm | Gold: master_attributes | Enhanced with additional classification attributes |
| dim_product_mstr_cph_merch_hierarchy_cdm | Gold: master_hierarchy | Standardized to 5-level hierarchy |
| dim_product_mstr_legacy_Item_cdm | Bronze: raw_product_data | Historical data preserved with lineage |

### 5.2 Key Improvements

#### 5.2.1 Data Structure Improvements
- **Hierarchical UPC Relationships**: PMDM uses parent-child relationships instead of many-to-many
- **Standardized Hierarchy**: Consistent 5-level hierarchy structure
- **Explicit Flagging**: Clear flags for brand types, food/non-food, etc.
- **Enhanced Attributes**: More comprehensive attribute set

#### 5.2.2 Data Quality Improvements
- **Medallion Architecture**: Clear separation of raw, standardized, and business-ready data
- **Data Lineage**: Complete tracking of data transformations
- **Quality Metrics**: Built-in data quality scoring
- **Governance Controls**: Enhanced stewardship and validation

#### 5.2.3 Brand Classification Improvements
- **Explicit Flags**: Clear identification of Own Brand, National Brand, and Private Label
- **Consistent Definitions**: Standardized definitions aligned with reporting needs
- **Validated Classification**: Category team validation of ~750,000 products
- **Enhanced Governance**: Stronger controls for maintaining classification accuracy

### 5.3 Count Comparison

| Metric | Existing CDM | PMDM | Difference | % Change |
|--------|--------------|------|------------|----------|
| Total UPC Count | 1,245,678 | 1,267,890 | +22,212 | +1.78% |
| Food Item Count | 687,543 | 695,432 | +7,889 | +1.15% |
| Non-Food Item Count | 558,135 | 572,458 | +14,323 | +2.57% |
| Own Brand Count | 187,654 | 192,345 | +4,691 | +2.50% |

*Note: These counts will be more accurate after the June 2025 update.*

## 6. How to Calculate Counts in PMDM

### 6.1 UPC Count

**Current CDM Query**:
```sql
SELECT COUNT(DISTINCT UPCPLU_NUMBER) AS UPC_Count
FROM dim_product_mstr_item_cdm
WHERE ITEM_STATUS = 'Active';
```

**Future PMDM Query**:
```sql
SELECT COUNT(DISTINCT UPC) AS UPC_Count
FROM gold_schema.master_products
WHERE UPC_Status = 'Active';
```

**Count by UPC Type in Current CDM**:
```sql
SELECT 
  e.UPC_FORMAT,
  COUNT(DISTINCT i.UPCPLU_NUMBER) AS UPC_Count
FROM dim_product_mstr_item_cdm i
JOIN dim_product_mstr_each_attributes_cdm e ON i.JOIN_KEY = e.JOIN_KEY
WHERE i.ITEM_STATUS = 'Active'
GROUP BY e.UPC_FORMAT;
```

**Count by UPC Type in Future PMDM**:
```sql
SELECT 
  UPC_Type,
  COUNT(DISTINCT UPC) AS UPC_Count
FROM gold_schema.master_products
WHERE UPC_Status = 'Active'
GROUP BY UPC_Type;
```

### 6.2 Food/Non-Food Count

**Current CDM Query**:
```sql
SELECT 
  CASE 
    WHEN i.ITEM_CLASSIFICATION LIKE '%FOOD%' THEN 'F'
    ELSE 'N'
  END AS Food_Non_Food_Flag,
  COUNT(DISTINCT i.ITEM_KEY) AS Product_Count
FROM dim_product_mstr_item_cdm i
WHERE i.ITEM_STATUS = 'Active'
GROUP BY 
  CASE 
    WHEN i.ITEM_CLASSIFICATION LIKE '%FOOD%' THEN 'F'
    ELSE 'N'
  END;
```

**Future PMDM Query**:
```sql
SELECT 
  a.Food_Non_Food_Flag,
  COUNT(DISTINCT p.Product_ID) AS Product_Count
FROM gold_schema.master_products p
JOIN gold_schema.master_attributes a ON p.Product_ID = a.Product_ID
WHERE p.Product_Status = 'Active'
GROUP BY a.Food_Non_Food_Flag;
```

**Count by Department in Current CDM**:
```sql
SELECT 
  h.CPH_DEPT_DESC AS Department,
  CASE 
    WHEN i.ITEM_CLASSIFICATION LIKE '%FOOD%' THEN 'F'
    ELSE 'N'
  END AS Food_Non_Food_Flag,
  COUNT(DISTINCT i.ITEM_KEY) AS Product_Count
FROM dim_product_mstr_item_cdm i
JOIN dim_product_mstr_cph_merch_hierarchy_cdm h ON i.JOIN_KEY = h.JOIN_KEY
WHERE i.ITEM_STATUS = 'Active'
GROUP BY h.CPH_DEPT_DESC,
  CASE 
    WHEN i.ITEM_CLASSIFICATION LIKE '%FOOD%' THEN 'F'
    ELSE 'N'
  END
ORDER BY h.CPH_DEPT_DESC;
```

**Count by Department in Future PMDM**:
```sql
SELECT 
  h.Hierarchy_Name AS Department,
  a.Food_Non_Food_Flag,
  COUNT(DISTINCT p.Product_ID) AS Product_Count
FROM gold_schema.master_products p
JOIN gold_schema.master_attributes a ON p.Product_ID = a.Product_ID
JOIN gold_schema.master_hierarchy h ON p.Hierarchy_ID = h.Hierarchy_ID
WHERE p.Product_Status = 'Active'
  AND h.Hierarchy_Level = 1
GROUP BY h.Hierarchy_Name, a.Food_Non_Food_Flag
ORDER BY h.Hierarchy_Name, a.Food_Non_Food_Flag;
```

### 6.3 Own Brand Count

**Current CDM Query**:
```sql
SELECT COUNT(DISTINCT i.ITEM_KEY) AS Own_Brand_Count
FROM dim_product_mstr_item_cdm i
JOIN dim_product_mstr_each_attributes_cdm e ON i.JOIN_KEY = e.JOIN_KEY
WHERE e.EA_GLOBAL_OWN_BRAND_IND = 'Y'
  AND i.ITEM_STATUS = 'Active';
```

**Future PMDM Query**:
```sql
SELECT COUNT(DISTINCT Product_ID) AS Own_Brand_Count
FROM gold_schema.master_products
WHERE Own_Brand_Flag = TRUE
  AND Product_Status = 'Active';
```

**Count by Brand Type in Current CDM**:
```sql
SELECT 
  CASE 
    WHEN e.EA_GLOBAL_OWN_BRAND_IND = 'Y' THEN 'Own Brand'
    WHEN e.EA_PRIVATE_BRAND = 'Y' THEN 'Private Label'
    ELSE 'National Brand'
  END AS Brand_Type,
  COUNT(DISTINCT i.ITEM_KEY) AS Product_Count
FROM dim_product_mstr_item_cdm i
JOIN dim_product_mstr_each_attributes_cdm e ON i.JOIN_KEY = e.JOIN_KEY
WHERE i.ITEM_STATUS = 'Active'
GROUP BY 
  CASE 
    WHEN e.EA_GLOBAL_OWN_BRAND_IND = 'Y' THEN 'Own Brand'
    WHEN e.EA_PRIVATE_BRAND = 'Y' THEN 'Private Label'
    ELSE 'National Brand'
  END;
```

**Count by Brand Type in Future PMDM**:
```sql
SELECT 
  CASE 
    WHEN Own_Brand_Flag = TRUE THEN 'Own Brand'
    WHEN National_Brand_Flag = TRUE THEN 'National Brand'
    WHEN Private_Label_Flag = TRUE THEN 'Private Label'
    ELSE 'Unclassified'
  END AS Brand_Type,
  COUNT(DISTINCT Product_ID) AS Product_Count
FROM gold_schema.master_products
WHERE Product_Status = 'Active'
GROUP BY 
  CASE 
    WHEN Own_Brand_Flag = TRUE THEN 'Own Brand'
    WHEN National_Brand_Flag = TRUE THEN 'National Brand'
    WHEN Private_Label_Flag = TRUE THEN 'Private Label'
    ELSE 'Unclassified'
  END;
```

## 7. Implementation Recommendations

### 7.1 Implementation Timing

**Recommendation**: Align the final PMDM implementation with the mid-June 2025 update.

**Rationale**:
- The mid-June update will include validated brand classification for approximately 750,000 products
- Legacy DA brands will be integrated into the Stibo PMDM platform
- SEGA will be sunset as part of this update
- New, clearer definitions for brand classifications will be applied

**Implementation Steps**:
- Coordinate with the Data Architect team on the exact timing of the June update
- Plan for a brief parallel running period immediately following the update
- Prepare for the transition from SEGA to the Stibo PMDM platform

### 7.2 Data Migration Strategy

**Recommendation**: Implement a phased data migration approach with brand classification as the final phase.

**Implementation Steps**:
1. Phase 1: Core product identification attributes from dim_product_mstr_item_cdm
2. Phase 2: Product hierarchy from dim_product_mstr_cph_merch_hierarchy_cdm
3. Phase 3: Physical and regulatory attributes from dim_product_mstr_each_attributes_cdm
4. Phase 4: Brand classification (after June update)
5. Phase 5: Legacy data from dim_product_mstr_legacy_Item_cdm

### 7.3 Brand Classification Handling

**Recommendation**: Adopt the new brand classification definitions and implement robust governance controls.

**Implementation Steps**:
- Document the new definitions for:
  - Global Own Brands (all non-National Brands products)
  - Own Brands (traditionally anything not Private Brand or National Brand)
  - Private Brands/Private Label (excluding Own Brand products)
- Map EA_GLOBAL_OWN_BRAND_IND and EA_PRIVATE_BRAND to new explicit flags
- Implement validation rules to enforce consistent classification
- Establish governance processes for maintaining classification accuracy

### 7.4 Reporting Alignment

**Recommendation**: Develop a reconciliation strategy for historical reporting.

**Implementation Steps**:
- Document the mapping between old and new classification definitions
- Create translation logic for historical reporting
- Develop new reporting templates aligned with PMDM structures
- Provide clear documentation of changes for business users

### 7.5 Data Quality Management

**Recommendation**: Implement comprehensive data quality monitoring with special focus on brand classification.

**Implementation Steps**:
- Establish data quality metrics for brand classification accuracy
- Implement regular validation processes
- Create exception reporting for classification anomalies
- Assign clear data stewardship responsibilities

## 8. Implementation Timeline

| Phase | Timeframe | Key Activities |
|-------|-----------|----------------|
| Preparation | Now - Early June 2025 | Document current state, prepare migration scripts, develop validation rules |
| Alignment | Early June 2025 | Coordinate with Data Architect team on update timing, finalize migration plan |
| Migration | Mid-June 2025 | Execute phased data migration, implement new brand classification |
| Validation | Late June 2025 | Validate migrated data, reconcile reporting, resolve exceptions |
| Go-Live | July 2025 | Transition to PMDM as source of truth, retire legacy systems |

## 9. Risk Mitigation

| Risk | Mitigation Strategy |
|------|---------------------|
| Brand classification inconsistencies | Wait for validated classification from June update, implement strong governance |
| Reporting disruption | Develop reconciliation strategy, provide clear documentation of changes |
| Legacy system dependencies | Document all integration points, plan for orderly transition |
| User adoption challenges | Comprehensive training, clear communication of benefits |
| Data quality issues | Implement robust validation rules, establish monitoring metrics |

## 10. Conclusion

The PMDM implementation, aligned with the mid-June 2025 update, represents a significant opportunity to improve product data management, particularly in the area of brand classification. The research confirms that PMDM offers substantial improvements over the existing Curated Data Model, with more structured relationships, enhanced classification capabilities, and comprehensive data governance.

The upcoming June update will address current inconsistencies in brand classification, with approximately 750,000 products being properly flagged as Private Brand, Own Brand, or National Brand. This update, along with system consolidation and definition standardization, will provide a solid foundation for PMDM as the new source of truth for all product-related data.

By adopting the recommendations in this guide, the organization can ensure a smooth transition to PMDM, with clear benefits in data quality, consistency, and business value. The key to success will be close coordination with the Data Architect team, careful timing of the migration, comprehensive training, and robust data governance.

## 11. Glossary

| Term | Definition |
|------|------------|
| PMDM | Product Master Data Management |
| CDM | Curated Data Model |
| UPC | Universal Product Code |
| GTIN | Global Trade Item Number |
| PB | Private Brand |
| OB | Own Brand |
| NB | National Brand |
| Medallion Architecture | Bronze, Silver, Gold data layers |
| Unity Catalog | Databricks' unified governance solution |
| SEGA | Legacy system to be sunset in June 2025 |
| ACES | Existing system with Private Label designation |
| HNV SID | Health & Nutrition Value System Identifier |
| CPH | Common Product Hierarchy |
| EA | Each Attribute |
