SELECT CF_SEQ,
       PROD_ID,
       PROD_DESC,
       CR_QTY,
       COLLECTION_DATE,
       CF_TYPE,
       SALE_NAME,
       WITHD_DATE,
       DAYS,
       CASE WHEN SALE_NAME = ' ' AND WITHD_DATE IS NULL THEN 1 ELSE 0 END
          STATE
  FROM (  SELECT CF.CF_SEQ,
                 SUBSTR (CF.PROD_ID, 7) || '-' || CF.REVISION PROD_ID,
                 P.PROD_DESC,
                 CR_QTY,
                 TO_CHAR (TRUNC (CF.COLLECTION_DATE), 'dd/mm/yyyy')
                    COLLECTION_DATE,
                 DECODE (
                    CF.CF_TYPE,
                    'N', 'ปกติ',
                    'A', 'แฟ้มสีพร้อมอาร์ตเวิร์คใหม่',
                    CF.CF_TYPE)
                    CF_TYPE,
                 EW.TITLE_NAME || EW.EMP_FNAME || ' ' || EW.EMP_LNAME SALE_NAME,
                 TO_CHAR (TRUNC (CF.WITHD_DATE), 'dd/mm/yyyy') WITHD_DATE,
                 TRUNC (SYSDATE) - TRUNC (CF.WITHD_DATE) DAYS
            FROM KPDBA.COLOR_FOLDER CF
                 JOIN KPDBA.PRODUCT P
                    ON CF.PROD_ID = P.PROD_ID AND CF.REVISION = P.REVISION
                 LEFT JOIN KPDBA.EMPLOYEE EW
                    ON CF.WITHD_USER_ID = EW.EMP_ID
           WHERE     NVL (CF.CANCEL_FLAG, 'F') = 'F'
                 AND NVL (CF.APPV_FLAG, 'P') = 'P'
        ORDER BY SUBSTR (CF.PROD_ID, 7), CF.REVISION)