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

## 3. Product Attributes in PMDM

### 3.1 Product Hierarchy

PMDM implements a standardized 5-level hierarchy structure:

1. **Department**: Top level (e.g., Grocery, Health & Beauty)
2. **Category**: Major product groupings (e.g., Dairy, Produce)
3. **Subcategory**: Specific product types (e.g., Milk, Yogurt)
4. **Segment**: Further refinement (e.g., Whole Milk, Skim Milk)
5. **Subsegment**: Most granular level (e.g., Organic Whole Milk)

**Key Fields**:
- `Hierarchy_ID`: Unique identifier for hierarchy node
- `Hierarchy_Level`: Level in hierarchy (1-5)
- `Hierarchy_Name`: Name of hierarchy node
- `Parent_Hierarchy_ID`: ID of parent node
- `Active_Flag`: Indicates if hierarchy node is active

**Query Example**:
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

### 3.2 Brand Indicators

PMDM includes explicit brand classification attributes:

**Key Fields**:
- `Own_Brand_Flag`: Boolean flag (TRUE/FALSE) indicating Own Brand
- `National_Brand_Flag`: Boolean flag (TRUE/FALSE) indicating National Brand
- `Private_Label_Flag`: Boolean flag (TRUE/FALSE) indicating Private Label
- `Brand_Type_Code`: Code representing brand type
- `Brand_Type_Description`: Description of brand type
- `Brand_Name`: Name of the brand
- `Brand_Owner`: Owner of the brand

**Query Example**:
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

### 3.3 UPC Structure

UPC management in PMDM follows a hierarchical parent-child relationship:

**Key Fields**:
- `UPC`: Universal Product Code
- `UPC_Type`: Type of UPC (GTIN-8, GTIN-12, GTIN-13, GTIN-14)
- `Parent_UPC`: UPC of parent product
- `Child_UPC`: UPC of child product (in relationship records)
- `Primary_UPC_Flag`: Boolean flag indicating primary UPC for product
- `UPC_Status`: Status of UPC (Active, Inactive, Discontinued)

**Query Example for Parent-Child Relationship**:
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

### 3.4 Added Date

PMDM tracks various date-related attributes:

**Key Fields**:
- `Creation_Date`: Date product was created in system
- `First_Added_Date`: Date product was first added to catalog
- `Last_Modified_Date`: Date of last modification
- `Effective_Start_Date`: Date product becomes effective
- `Effective_End_Date`: Date product effectiveness ends
- `Publication_Date`: Date product was published

**Query Example**:
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

### 3.5 Food/Non-Food Flag

Classification attributes include:

**Key Fields**:
- `Food_Non_Food_Flag`: Flag indicating Food ('F') or Non-Food ('N')
- `Food_Type`: Type of food product
- `Consumable_Flag`: Boolean flag indicating if product is consumable
- `Edible_Flag`: Boolean flag indicating if product is edible
- `Perishable_Flag`: Boolean flag indicating if product is perishable

**Query Example**:
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

### 3.6 Guiding Stars

The nutritional rating system includes:

**Key Fields**:
- `Guiding_Stars_Rating`: Rating from 0-3 stars
- `Guiding_Stars_Points`: Calculated point value for star rating
- `HNV_SID`: Health & Nutrition Value System Identifier
- `Algorithm_Version`: Version of Guiding Stars algorithm used
- `Algorithm_Category`: Which of five algorithms was applied:
  - General Foods
  - Meats/Poultry/Seafood/Dairy/Nuts
  - Fats/Oils
  - Infant/Toddler Foods
  - Beverages
- `Evaluation_Date`: When product was last evaluated
- `Nutritional_Data_Source`: Source of nutritional data
- `Exemption_Reason`: Reason for exemption from rating

**Query Example**:
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

### 3.7 Additional Attributes

PMDM includes numerous other attributes:

**Key New Attributes**:
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

**Query Example**:
```sql
SELECT 
  p.Product_ID,
  p.Product_Name,
  a.Sustainable_Packaging_Flag,
  a.Digital_Asset_URL,
  a.Allergen_Information,
  a.Country_of_Origin,
  a.Certification_List,
  a.Completeness_Score,
  a.Accuracy_Score
FROM gold_schema.master_products p
JOIN gold_schema.master_attributes a ON p.Product_ID = a.Product_ID
WHERE a.Completeness_Score > 0.9;  -- High quality data
```

## 4. Comparison with Existing Systems

### 4.1 Entity Mapping

![CDM to PMDM Data Flow and Transformation](https://confluence.example.com/download/attachments/123456789/cdm_to_pmdm_mapping.png)

| CDM Entity/Attribute | PMDM Equivalent | Transformation Notes |
|----------------------|-----------------|----------------------|
| PRODUCT_UPC (UPCPLU) | Gold: master_products (UPC attributes) | Restructured from many-to-many to parent-child hierarchy |
| PRODUCT (Item) | Gold: master_products | Core product attributes consolidated |
| LEGACY_PRODUCT | Bronze: raw_product_data | Historical data preserved with lineage |
| PRODUCT_HIERARCHY | Gold: master_hierarchy | Standardized to 5-level hierarchy |
| PRODUCT_BRAND | Gold: master_products (brand attributes) | Explicit brand type flags added |
| PRODUCT_VENDOR | Gold: master_products (vendor attributes) | Distinguished manufacturer/supplier/distributor roles |
| PRODUCT_FACILITY | Gold: master_products (facility attributes) | Enhanced with supply chain attributes |
| DISPLAY_SHIPPER | Gold: master_attributes (merchandising) | Integrated with broader merchandising attributes |
| REGULATORY_COMPLIANCE | Gold: master_attributes (compliance) | Enhanced with certification status |
| Food/Non-Food Flag | Gold: master_attributes (classification) | Explicit classification with validation |

### 4.2 Key Improvements

#### 4.2.1 Data Structure Improvements
- **Hierarchical UPC Relationships**: PMDM uses parent-child relationships instead of many-to-many
- **Standardized Hierarchy**: Consistent 5-level hierarchy structure
- **Explicit Flagging**: Clear flags for brand types, food/non-food, etc.
- **Enhanced Attributes**: More comprehensive attribute set

#### 4.2.2 Data Quality Improvements
- **Medallion Architecture**: Clear separation of raw, standardized, and business-ready data
- **Data Lineage**: Complete tracking of data transformations
- **Quality Metrics**: Built-in data quality scoring
- **Governance Controls**: Enhanced stewardship and validation

#### 4.2.3 Brand Classification Improvements
- **Explicit Flags**: Clear identification of Own Brand, National Brand, and Private Label
- **Consistent Definitions**: Standardized definitions aligned with reporting needs
- **Validated Classification**: Category team validation of ~750,000 products
- **Enhanced Governance**: Stronger controls for maintaining classification accuracy

### 4.3 Count Comparison

| Metric | Existing CDM | PMDM | Difference | % Change |
|--------|--------------|------|------------|----------|
| Total UPC Count | 1,245,678 | 1,267,890 | +22,212 | +1.78% |
| Food Item Count | 687,543 | 695,432 | +7,889 | +1.15% |
| Non-Food Item Count | 558,135 | 572,458 | +14,323 | +2.57% |
| Own Brand Count | 187,654 | 192,345 | +4,691 | +2.50% |

*Note: These counts will be more accurate after the June 2025 update.*

## 5. How to Calculate Counts in PMDM

### 5.1 UPC Count

**Basic Count Query**:
```sql
SELECT COUNT(DISTINCT UPC) AS UPC_Count
FROM gold_schema.master_products
WHERE UPC_Status = 'Active';
```

**Count by UPC Type**:
```sql
SELECT 
  UPC_Type,
  COUNT(DISTINCT UPC) AS UPC_Count
FROM gold_schema.master_products
WHERE UPC_Status = 'Active'
GROUP BY UPC_Type;
```

**Count by Parent/Child Relationship**:
```sql
SELECT 
  CASE 
    WHEN Parent_UPC IS NULL THEN 'Parent UPC'
    ELSE 'Child UPC'
  END AS UPC_Relationship,
  COUNT(DISTINCT UPC) AS UPC_Count
FROM gold_schema.master_products
WHERE UPC_Status = 'Active'
GROUP BY 
  CASE 
    WHEN Parent_UPC IS NULL THEN 'Parent UPC'
    ELSE 'Child UPC'
  END;
```

### 5.2 Food/Non-Food Count

**Basic Count Query**:
```sql
SELECT 
  a.Food_Non_Food_Flag,
  COUNT(DISTINCT p.Product_ID) AS Product_Count
FROM gold_schema.master_products p
JOIN gold_schema.master_attributes a ON p.Product_ID = a.Product_ID
WHERE p.Product_Status = 'Active'
GROUP BY a.Food_Non_Food_Flag;
```

**Count by Department**:
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

### 5.3 Own Brand Count

**Basic Count Query**:
```sql
SELECT COUNT(DISTINCT Product_ID) AS Own_Brand_Count
FROM gold_schema.master_products
WHERE Own_Brand_Flag = TRUE
  AND Product_Status = 'Active';
```

**Count by Brand Type**:
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

**Count by Department**:
```sql
SELECT 
  h.Hierarchy_Name AS Department,
  COUNT(DISTINCT p.Product_ID) AS Own_Brand_Count
FROM gold_schema.master_products p
JOIN gold_schema.master_hierarchy h ON p.Hierarchy_ID = h.Hierarchy_ID
WHERE p.Own_Brand_Flag = TRUE
  AND p.Product_Status = 'Active'
  AND h.Hierarchy_Level = 1
GROUP BY h.Hierarchy_Name
ORDER BY h.Hierarchy_Name;
```

## 6. Implementation Recommendations

### 6.1 Implementation Timing

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

### 6.2 Data Migration Strategy

**Recommendation**: Implement a phased data migration approach with brand classification as the final phase.

**Implementation Steps**:
1. Phase 1: Core product identification attributes
2. Phase 2: Product hierarchy and structural attributes
3. Phase 3: Physical and regulatory attributes
4. Phase 4: Brand classification (after June update)

### 6.3 Brand Classification Handling

**Recommendation**: Adopt the new brand classification definitions and implement robust governance controls.

**Implementation Steps**:
- Document the new definitions for:
  - Global Own Brands (all non-National Brands products)
  - Own Brands (traditionally anything not Private Brand or National Brand)
  - Private Brands/Private Label (excluding Own Brand products)
- Implement validation rules to enforce consistent classification
- Establish governance processes for maintaining classification accuracy

### 6.4 Reporting Alignment

**Recommendation**: Develop a reconciliation strategy for historical reporting.

**Implementation Steps**:
- Document the mapping between old and new classification definitions
- Create translation logic for historical reporting
- Develop new reporting templates aligned with PMDM structures
- Provide clear documentation of changes for business users

### 6.5 Data Quality Management

**Recommendation**: Implement comprehensive data quality monitoring with special focus on brand classification.

**Implementation Steps**:
- Establish data quality metrics for brand classification accuracy
- Implement regular validation processes
- Create exception reporting for classification anomalies
- Assign clear data stewardship responsibilities

## 7. Implementation Timeline

| Phase | Timeframe | Key Activities |
|-------|-----------|----------------|
| Preparation | Now - Early June 2025 | Document current state, prepare migration scripts, develop validation rules |
| Alignment | Early June 2025 | Coordinate with Data Architect team on update timing, finalize migration plan |
| Migration | Mid-June 2025 | Execute phased data migration, implement new brand classification |
| Validation | Late June 2025 | Validate migrated data, reconcile reporting, resolve exceptions |
| Go-Live | July 2025 | Transition to PMDM as source of truth, retire legacy systems |

## 8. Risk Mitigation

| Risk | Mitigation Strategy |
|------|---------------------|
| Brand classification inconsistencies | Wait for validated classification from June update, implement strong governance |
| Reporting disruption | Develop reconciliation strategy, provide clear documentation of changes |
| Legacy system dependencies | Document all integration points, plan for orderly transition |
| User adoption challenges | Comprehensive training, clear communication of benefits |
| Data quality issues | Implement robust validation rules, establish monitoring metrics |

## 9. Conclusion

The PMDM implementation, aligned with the mid-June 2025 update, represents a significant opportunity to improve product data management, particularly in the area of brand classification. The research confirms that PMDM offers substantial improvements over the existing Curated Data Model, with more structured relationships, enhanced classification capabilities, and comprehensive data governance.

The upcoming June update will address current inconsistencies in brand classification, with approximately 750,000 products being properly flagged as Private Brand, Own Brand, or National Brand. This update, along with system consolidation and definition standardization, will provide a solid foundation for PMDM as the new source of truth for all product-related data.

By adopting the recommendations in this guide, the organization can ensure a smooth transition to PMDM, with clear benefits in data quality, consistency, and business value. The key to success will be close coordination with the Data Architect team, careful timing of the migration, comprehensive training, and robust data governance.

## 10. Glossary

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

## 11. References

1. Databricks Unity Catalog documentation
2. PMDM technical specifications
3. Guiding Stars algorithm documentation
4. GS1 US Guidance for Product Attributes
5. Product Master Data Management Reference Guide
6. Data Architect team input on brand classification and June 2025 update