# Forecast Domain Star Schema Specifications with Updated Naming Conventions

This document provides detailed specifications for the Forecast domain star schema, designed to be generalized across different brands. The naming conventions have been updated to use the format `dim_*_cdm` for dimension tables and `fact_*_cdm` for fact tables.

## Fact Tables Specifications

### 1. fact_volume_forecast_cdm

**Purpose**: Captures volume forecasts and actuals to enable comparison and variance analysis.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| volume_forecast_key | BIGINT | Surrogate primary key | Generated |
| date_key | INT | Foreign key to dim_date_cdm | partitionDate |
| category_key | INT | Foreign key to dim_category_cdm | genericCategoryId |
| department_key | INT | Foreign key to dim_department_cdm | genericDepartmentId |
| brand_key | INT | Foreign key to dim_brand_cdm | Derived from view name |
| source_key | INT | Foreign key to dim_source_cdm | Derived |
| forecast_amount | DECIMAL(12,2) | Forecasted volume amount | amount |
| actual_amount | DECIMAL(12,2) | Actual volume amount | dailyAmount |
| variance_amount | DECIMAL(12,2) | Difference between forecast and actual | Calculated |
| variance_percentage | DECIMAL(8,4) | Percentage variance | Calculated |
| volume_driver_id | VARCHAR(50) | Identifier for the volume driver | volumeDriverId |
| post_label | VARCHAR(100) | Post label for the forecast | postLabel |
| post_label_id | VARCHAR(50) | Identifier for the post label | postLabelId |
| increment_cnt | INT | Increment count | incrementCnt |
| linked_category_type | VARCHAR(50) | Type of linked category | linkedCategoryType |
| include_summary_set | VARCHAR(1) | Flag for inclusion in summary | includeSummarySet |
| create_user_id | VARCHAR(50) | User who created the record | record_creation_user_id |
| create_timestamp | TIMESTAMP | When record was created | record_creation_timestamp |
| update_user_id | VARCHAR(50) | User who last updated the record | last_update_user_id |
| update_timestamp | TIMESTAMP | When record was last updated | record_last_update_timestamp |

**Primary Key**: volume_forecast_key

**Foreign Keys**:
- date_key references dim_date_cdm(date_key)
- category_key references dim_category_cdm(category_key)
- department_key references dim_department_cdm(department_key)
- brand_key references dim_brand_cdm(brand_key)
- source_key references dim_source_cdm(source_key)

**Indexes**:
- Primary key index on volume_forecast_key
- Composite index on (date_key, brand_key)
- Composite index on (category_key, date_key)
- Composite index on (department_key, date_key)

### 2. fact_labor_forecast_cdm

**Purpose**: Captures labor forecasts to enable labor planning and analysis.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| labor_forecast_key | BIGINT | Surrogate primary key | Generated |
| date_key | INT | Foreign key to dim_date_cdm | partitionDate |
| labor_standard_key | INT | Foreign key to dim_labor_standard_cdm | laborStandardId |
| department_key | INT | Foreign key to dim_department_cdm | genericDepartmentId |
| brand_key | INT | Foreign key to dim_brand_cdm | Derived from view name |
| source_key | INT | Foreign key to dim_source_cdm | Derived |
| forecast_type_key | INT | Foreign key to dim_forecast_type_cdm | laborForecastTypeName |
| daily_hrs | DECIMAL(8,2) | Daily hours forecasted | dailyHrs |
| start_time | TIME | Start time for the forecast | startTime |
| end_time | TIME | End time for the forecast | endTime |
| increment_cnt | INT | Increment count | incrementCnt |
| update_dttm | TIMESTAMP | Last update timestamp | updateDttm |
| create_user_id | VARCHAR(50) | User who created the record | record_creation_user_id |
| create_timestamp | TIMESTAMP | When record was created | record_creation_timestamp |
| update_user_id | VARCHAR(50) | User who last updated the record | last_update_user_id |
| update_timestamp | TIMESTAMP | When record was last updated | record_last_update_timestamp |

**Primary Key**: labor_forecast_key

**Foreign Keys**:
- date_key references dim_date_cdm(date_key)
- labor_standard_key references dim_labor_standard_cdm(labor_standard_key)
- department_key references dim_department_cdm(department_key)
- brand_key references dim_brand_cdm(brand_key)
- source_key references dim_source_cdm(source_key)
- forecast_type_key references dim_forecast_type_cdm(forecast_type_key)

**Indexes**:
- Primary key index on labor_forecast_key
- Composite index on (date_key, brand_key)
- Composite index on (labor_standard_key, date_key)
- Composite index on (forecast_type_key, date_key)

### 3. fact_custom_driver_cdm

**Purpose**: Captures custom driver values used in forecasting.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| custom_driver_key | BIGINT | Surrogate primary key | Generated |
| date_key | INT | Foreign key to dim_date_cdm | partitionDate |
| brand_key | INT | Foreign key to dim_brand_cdm | Derived from view name |
| source_key | INT | Foreign key to dim_source_cdm | Derived |
| driver_key | INT | Foreign key to dim_driver_cdm | cstmDriverId |
| amount | DECIMAL(12,2) | Driver amount value | amount |
| currency_id | VARCHAR(10) | Currency identifier | currencyId |
| version | VARCHAR(20) | Version of the driver | version |
| update_dttm | TIMESTAMP | Last update timestamp | updateDttm |
| create_user_id | VARCHAR(50) | User who created the record | record_creation_user_id |
| create_timestamp | TIMESTAMP | When record was created | record_creation_timestamp |
| update_user_id | VARCHAR(50) | User who last updated the record | last_update_user_id |
| update_timestamp | TIMESTAMP | When record was last updated | record_last_update_timestamp |

**Primary Key**: custom_driver_key

**Foreign Keys**:
- date_key references dim_date_cdm(date_key)
- brand_key references dim_brand_cdm(brand_key)
- source_key references dim_source_cdm(source_key)
- driver_key references dim_driver_cdm(driver_key)

**Indexes**:
- Primary key index on custom_driver_key
- Composite index on (date_key, brand_key)
- Composite index on (driver_key, date_key)

### 4. fact_labor_standard_task_cdm

**Purpose**: Links labor standards to tasks and captures task-specific labor metrics.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| labor_standard_task_key | BIGINT | Surrogate primary key | Generated |
| labor_standard_key | INT | Foreign key to dim_labor_standard_cdm | laborStandardId |
| task_key | INT | Foreign key to dim_task_cdm | taskId |
| task_group_key | INT | Foreign key to dim_task_group_cdm | taskGroupId |
| brand_key | INT | Foreign key to dim_brand_cdm | Derived from view name |
| source_key | INT | Foreign key to dim_source_cdm | Derived |
| total_labor_minutes | DECIMAL(8,2) | Total labor minutes for the task | totalLaborMinutes |
| combined_labor_distribution_id | VARCHAR(50) | ID for combined labor distribution | combinedLaborDistributionId |
| time_dependent_swt | CHAR(1) | Flag for time dependency | timeDependentSwt |
| allocate_extra_labor_before_traffic_swt | CHAR(1) | Flag for labor allocation | allocateExtraLaborBeforeTrafficSwt |
| selected_for_combined_distribution_swt | CHAR(1) | Flag for combined distribution | selectedForCombinedDistributionSwt |
| store_specific_swt | CHAR(1) | Flag for store-specific task | storeSpecificSwt |
| update_dttm | TIMESTAMP | Last update timestamp | updateDttm |
| create_user_id | VARCHAR(50) | User who created the record | record_creation_user_id |
| create_timestamp | TIMESTAMP | When record was created | record_creation_timestamp |
| update_user_id | VARCHAR(50) | User who last updated the record | last_update_user_id |
| update_timestamp | TIMESTAMP | When record was last updated | record_last_update_timestamp |

**Primary Key**: labor_standard_task_key

**Foreign Keys**:
- labor_standard_key references dim_labor_standard_cdm(labor_standard_key)
- task_key references dim_task_cdm(task_key)
- task_group_key references dim_task_group_cdm(task_group_key)
- brand_key references dim_brand_cdm(brand_key)
- source_key references dim_source_cdm(source_key)

**Indexes**:
- Primary key index on labor_standard_task_key
- Composite index on (labor_standard_key, task_key)
- Composite index on (task_group_key, task_key)
- Composite index on (brand_key, labor_standard_key)

### 5. fact_labor_standard_driver_cdm

**Purpose**: Links labor standards to drivers and captures driver-specific labor metrics.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| labor_standard_driver_key | BIGINT | Surrogate primary key | Generated |
| labor_standard_key | INT | Foreign key to dim_labor_standard_cdm | laborStandardId |
| driver_key | INT | Foreign key to dim_driver_cdm | volumeDriverId |
| brand_key | INT | Foreign key to dim_brand_cdm | Derived from view name |
| source_key | INT | Foreign key to dim_source_cdm | Derived |
| effective_version_id | VARCHAR(50) | Effective version identifier | effectiveVersionId |
| look_back_days | INT | Number of look-back days | lookBackDays |
| frequency | DECIMAL(8,2) | Frequency value | frequency |
| frequency_unit | VARCHAR(20) | Unit of frequency | frequencyUnit |
| application_factor | DECIMAL(8,4) | Application factor | applicationFactor |
| volume_driver_id | VARCHAR(50) | Volume driver identifier | volumeDriverId |
| secondary_volume_driver_id | VARCHAR(50) | Secondary volume driver identifier | secondaryVolumeDriverId |
| update_dttm | TIMESTAMP | Last update timestamp | updateDttm |
| create_user_id | VARCHAR(50) | User who created the record | record_creation_user_id |
| create_timestamp | TIMESTAMP | When record was created | record_creation_timestamp |
| update_user_id | VARCHAR(50) | User who last updated the record | last_update_user_id |
| update_timestamp | TIMESTAMP | When record was last updated | record_last_update_timestamp |

**Primary Key**: labor_standard_driver_key

**Foreign Keys**:
- labor_standard_key references dim_labor_standard_cdm(labor_standard_key)
- driver_key references dim_driver_cdm(driver_key)
- brand_key references dim_brand_cdm(brand_key)
- source_key references dim_source_cdm(source_key)

**Indexes**:
- Primary key index on labor_standard_driver_key
- Composite index on (labor_standard_key, driver_key)
- Composite index on (brand_key, labor_standard_key)

### 6. fact_labor_standard_period_cdm

**Purpose**: Captures period-specific labor standard metrics.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| labor_standard_period_key | BIGINT | Surrogate primary key | Generated |
| labor_standard_key | INT | Foreign key to dim_labor_standard_cdm | laborStandardId |
| brand_key | INT | Foreign key to dim_brand_cdm | Derived from view name |
| source_key | INT | Foreign key to dim_source_cdm | Derived |
| effective_version_id | VARCHAR(50) | Effective version identifier | effectiveVersionId |
| effective_date | DATE | Effective date | effectiveDate |
| expiration_date | DATE | Expiration date | expirationDate |
| distribution_type | VARCHAR(50) | Type of distribution | distributionType |
| remainder_distribution | VARCHAR(50) | Remainder distribution method | remainderDistribution |
| volume_driver_id | VARCHAR(50) | Volume driver identifier | volumeDriverId |
| distribution_traffic_pattern_swt | CHAR(1) | Flag for traffic pattern | distributionTrafficPatternSwt |
| accumulated_driver_swt | CHAR(1) | Flag for accumulated driver | accumulatedDriverSwt |
| override_pos_swt | CHAR(1) | Flag for POS override | overridePosSwt |
| application_type | VARCHAR(50) | Type of application | applicationType |
| day_of_week | INT | Day of week | dayOfWeek |
| offset_min | INT | Minimum offset | offsetMin |
| offset_minutes | INT | Offset in minutes | offsetMinutes |
| offset_duration | INT | Duration of offset | offsetDuration |
| update_dttm | TIMESTAMP | Last update timestamp | updateDttm |
| create_user_id | VARCHAR(50) | User who created the record | record_creation_user_id |
| create_timestamp | TIMESTAMP | When record was created | record_creation_timestamp |
| update_user_id | VARCHAR(50) | User who last updated the record | last_update_user_id |
| update_timestamp | TIMESTAMP | When record was last updated | record_last_update_timestamp |

**Primary Key**: labor_standard_period_key

**Foreign Keys**:
- labor_standard_key references dim_labor_standard_cdm(labor_standard_key)
- brand_key references dim_brand_cdm(brand_key)
- source_key references dim_source_cdm(source_key)

**Indexes**:
- Primary key index on labor_standard_period_key
- Composite index on (labor_standard_key, effective_date)
- Composite index on (brand_key, labor_standard_key)
- Index on effective_date
- Index on expiration_date

## Dimension Tables Specifications

### 1. dim_date_cdm

**Purpose**: Provides time-based attributes for analysis.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| date_key | INT | Surrogate primary key | Generated |
| full_date | DATE | Full date | partitionDate |
| year | INT | Year | Derived |
| quarter | INT | Quarter (1-4) | Derived |
| month | INT | Month (1-12) | Derived |
| month_name | VARCHAR(10) | Month name | Derived |
| week | INT | Week of year | Derived |
| day | INT | Day of month | Derived |
| day_of_week | INT | Day of week (1-7) | Derived |
| day_name | VARCHAR(10) | Day name | Derived |
| is_weekend | BOOLEAN | Flag for weekend | Derived |
| is_holiday | BOOLEAN | Flag for holiday | Derived |
| fiscal_year | INT | Fiscal year | Derived |
| fiscal_quarter | INT | Fiscal quarter | Derived |
| fiscal_month | INT | Fiscal month | Derived |
| fiscal_week | INT | Fiscal week | Derived |

**Primary Key**: date_key

**Indexes**:
- Primary key index on date_key
- Index on full_date
- Index on (year, month, day)
- Index on fiscal_year

### 2. dim_brand_cdm

**Purpose**: Provides brand-specific attributes for cross-brand analysis.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| brand_key | INT | Surrogate primary key | Generated |
| brand_id | VARCHAR(50) | Business identifier for brand | Derived from view name |
| brand_name | VARCHAR(100) | Brand name | Derived from view name |
| brand_code | VARCHAR(20) | Brand code | Derived |
| region | VARCHAR(50) | Geographic region | Derived |
| division | VARCHAR(50) | Business division | Derived |
| active_swt | CHAR(1) | Active flag | activeSwt |
| effective_date | DATE | Effective date | Derived |
| expiration_date | DATE | Expiration date | Derived |

**Primary Key**: brand_key

**Indexes**:
- Primary key index on brand_key
- Unique index on brand_id
- Index on brand_name

### 3. dim_department_cdm

**Purpose**: Provides department-level attributes for organizational hierarchy.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| department_key | INT | Surrogate primary key | Generated |
| generic_department_id | VARCHAR(50) | Business identifier for department | genericDepartmentId |
| generic_department_name | VARCHAR(100) | Department name | genericDepartmentName |
| department_type | VARCHAR(50) | Type of department | Derived |
| active_swt | CHAR(1) | Active flag | activeSwt |
| site_path_txt | VARCHAR(255) | Site path text | sitePathTxt |
| effective_date | DATE | Effective date | effectiveDate |
| expiration_date | DATE | Expiration date | Derived |

**Primary Key**: department_key

**Indexes**:
- Primary key index on department_key
- Unique index on generic_department_id
- Index on generic_department_name

### 4. dim_category_cdm

**Purpose**: Provides category-level attributes for product categorization.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| category_key | INT | Surrogate primary key | Generated |
| generic_category_id | VARCHAR(50) | Business identifier for category | genericCategoryId |
| generic_category_name | VARCHAR(100) | Category name | genericCategoryName |
| department_key | INT | Foreign key to dim_department_cdm | Derived |
| category_type | VARCHAR(50) | Type of category | Derived |
| active_swt | CHAR(1) | Active flag | activeSwt |
| effective_date | DATE | Effective date | effectiveDate |
| expiration_date | DATE | Expiration date | Derived |

**Primary Key**: category_key

**Foreign Keys**:
- department_key references dim_department_cdm(department_key)

**Indexes**:
- Primary key index on category_key
- Unique index on generic_category_id
- Index on generic_category_name
- Index on department_key

### 5. dim_labor_standard_cdm

**Purpose**: Provides labor standard reference data.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| labor_standard_key | INT | Surrogate primary key | Generated |
| labor_standard_id | VARCHAR(50) | Business identifier for labor standard | laborStandardId |
| labor_standard_name | VARCHAR(100) | Labor standard name | laborStandardName |
| labor_standard_description | VARCHAR(255) | Description of labor standard | laborStandardDescription |
| active_swt | CHAR(1) | Active flag | activeSwt |
| version | VARCHAR(20) | Version of the standard | version |
| effective_date | DATE | Effective date | effectiveDate |
| expiration_date | DATE | Expiration date | expirationDate |

**Primary Key**: labor_standard_key

**Indexes**:
- Primary key index on labor_standard_key
- Unique index on labor_standard_id
- Index on labor_standard_name

### 6. dim_task_cdm

**Purpose**: Provides task reference data.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| task_key | INT | Surrogate primary key | Generated |
| task_id | VARCHAR(50) | Business identifier for task | taskId |
| task_name | VARCHAR(100) | Task name | taskName |
| task_description | VARCHAR(255) | Description of task | taskDescription |
| active_swt | CHAR(1) | Active flag | activeSwt |
| version | VARCHAR(20) | Version of the task | version |
| effective_date | DATE | Effective date | effectiveDate |
| expiration_date | DATE | Expiration date | expirationDate |

**Primary Key**: task_key

**Indexes**:
- Primary key index on task_key
- Unique index on task_id
- Index on task_name

### 7. dim_task_group_cdm

**Purpose**: Provides task group reference data.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| task_group_key | INT | Surrogate primary key | Generated |
| task_group_id | VARCHAR(50) | Business identifier for task group | taskGroupId |
| task_group_name | VARCHAR(100) | Task group name | taskGroupName |
| active_swt | CHAR(1) | Active flag | activeSwt |
| version | VARCHAR(20) | Version of the task group | version |
| effective_date | DATE | Effective date | effectiveDate |
| expiration_date | DATE | Expiration date | expirationDate |

**Primary Key**: task_group_key

**Indexes**:
- Primary key index on task_group_key
- Unique index on task_group_id
- Index on task_group_name

### 8. dim_driver_cdm

**Purpose**: Provides driver reference data.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| driver_key | INT | Surrogate primary key | Generated |
| volume_driver_id | VARCHAR(50) | Business identifier for driver | volumeDriverId |
| volume_driver_name | VARCHAR(100) | Driver name | volumeDriver |
| driver_type | VARCHAR(50) | Type of driver | Derived |
| active_swt | CHAR(1) | Active flag | activeSwt |
| version | VARCHAR(20) | Version of the driver | version |
| effective_date | DATE | Effective date | effectiveDate |
| expiration_date | DATE | Expiration date | expirationDate |

**Primary Key**: driver_key

**Indexes**:
- Primary key index on driver_key
- Unique index on volume_driver_id
- Index on volume_driver_name

### 9. dim_forecast_type_cdm

**Purpose**: Provides forecast type reference data.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| forecast_type_key | INT | Surrogate primary key | Generated |
| labor_forecast_type_name | VARCHAR(100) | Forecast type name | laborForecastTypeName |
| forecast_type_description | VARCHAR(255) | Description of forecast type | Derived |
| active_swt | CHAR(1) | Active flag | activeSwt |
| version | VARCHAR(20) | Version of the forecast type | version |
| effective_date | DATE | Effective date | effectiveDate |
| expiration_date | DATE | Expiration date | expirationDate |

**Primary Key**: forecast_type_key

**Indexes**:
- Primary key index on forecast_type_key
- Unique index on labor_forecast_type_name

### 10. dim_source_cdm

**Purpose**: Provides data source reference information.

**Attributes**:
| Attribute | Data Type | Description | Source Field |
|-----------|-----------|-------------|--------------|
| source_key | INT | Surrogate primary key | Generated |
| source_id | VARCHAR(50) | Business identifier for source | Derived |
| source_name | VARCHAR(100) | Source name | Derived |
| source_type | VARCHAR(50) | Type of source | Derived |
| source_description | VARCHAR(255) | Description of source | Derived |
| active_swt | CHAR(1) | Active flag | Derived |
| effective_date | DATE | Effective date | Derived |
| expiration_date | DATE | Expiration date | Derived |

**Primary Key**: source_key

**Indexes**:
- Primary key index on source_key
- Unique index on source_id
- Index on source_name

## Data Loading Strategy

1. **Initial Load**:
   - Load dimension tables first, starting with dim_date_cdm and dim_brand_cdm
   - Generate surrogate keys for all dimensions
   - Load fact tables with appropriate foreign key references

2. **Incremental Load**:
   - Use partitionDate to identify new or changed records
   - Apply SCD Type 2 for dimension tables where historical tracking is needed
   - Append new fact records and update existing ones based on business keys

3. **Brand Generalization**:
   - Extract brand information from view names during ETL
   - Populate dim_brand_cdm with all brands
   - Transform brand-specific fields to use brand_key references

## Data Refresh Frequency

| Table Type | Refresh Frequency | Method |
|------------|-------------------|--------|
| Dimension Tables | Daily | Full refresh for small dimensions, incremental for large ones |
| Fact Tables | Daily | Incremental load based on partitionDate |
| dim_date_cdm | Monthly | Add future dates |

## Data Quality Rules

1. **Referential Integrity**:
   - All foreign keys must reference valid primary keys
   - No orphaned fact records

2. **Data Completeness**:
   - Required fields must not be null (e.g., date_key, brand_key)
   - Fact measures should have appropriate default values when null

3. **Data Consistency**:
   - Date ranges must be valid (effective_date <= expiration_date)
   - Version numbers should follow consistent pattern

4. **Data Accuracy**:
   - Numeric measures must be within valid ranges
   - Calculated fields must be verified against source

## Performance Optimization

1. **Partitioning**:
   - Partition fact tables by date_key
   - Consider brand_key as a secondary partition for large deployments

2. **Indexing**:
   - Create appropriate indexes as specified for each table
   - Consider columnar storage for analytical queries

3. **Aggregation**:
   - Create pre-aggregated summary tables for common queries
   - Consider materialized views for frequently accessed metrics

4. **Query Optimization**:
   - Optimize join paths through star schema design
   - Use appropriate filtering on dimension attributes

## Security and Access Control

1. **Row-Level Security**:
   - Implement brand-level security using dim_brand_cdm
   - Consider department-level security for organizational restrictions

2. **Column-Level Security**:
   - Restrict access to sensitive metrics based on user roles
   - Mask or encrypt sensitive attributes

3. **Audit Trail**:
   - Maintain create/update user and timestamp fields
   - Log all data access and modifications

This specification provides a comprehensive blueprint for implementing the Forecast domain star schema in a way that can be generalized across different brands while maintaining the relationships and metrics needed for forecast analytics.
