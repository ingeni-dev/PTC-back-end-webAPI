  SELECT SUBSTR (CF.PROD_ID, 7) || '-' || CF.REVISION PROD_ID,
         P.PROD_DESC,
         CASE WHEN SN.CF_NO IS NULL THEN NULL ELSE TO_CHAR (SN.CF_NO) || '/' || TO_CHAR (CF.SUBMIT_QTY) END SN_NO,
         MCS.CF_STATUS_DESC,
         SD.LOC_DETAIL,
         TO_CHAR (TRUNC (SD.TRAN_DATE), 'dd/mm/yyyy') TRAN_DATE,
         SN.CF_SN
    FROM KPDBA.COLOR_FOLDER CF
         JOIN KPDBA.COLOR_FOLDER_SN SN
            ON CF.CF_SEQ = SN.CF_SEQ
         JOIN KPDBA.PRODUCT P
            ON CF.PROD_ID = P.PROD_ID AND CF.REVISION = P.REVISION
         JOIN KPDBA.CF_STATUS_MASTER MCS
            ON SN.CF_STATUS = MCS.CF_STATUS
         LEFT JOIN (  SELECT SD.CF_SN,
                             SD.WAREHOUSE_ID,
                             NVL (
                                LOC.LOC_DETAIL,
                                E.TITLE_NAME || E.EMP_FNAME || ' ' || E.EMP_LNAME)
                                LOC_DETAIL,
                             MAX (SD.TRAN_DATE) TRAN_DATE
                        FROM KPDBA.CF_STOCK_DETAIL SD
                             LEFT JOIN KPDBA.LOCATION_CF LOC
                                ON     SD.WAREHOUSE_ID = LOC.WAREHOUSE_ID
                                   AND SD.LOC_ID = LOC.LOC_ID
                             LEFT JOIN KPDBA.EMPLOYEE E
                                ON SD.EMP_ID = E.EMP_ID
                       WHERE SD.STATUS = 'T'
                    GROUP BY SD.CF_SN,
                             SD.WAREHOUSE_ID,
                             LOC.LOC_DETAIL,
                             E.TITLE_NAME || E.EMP_FNAME || ' ' || E.EMP_LNAME
                      HAVING SUM (SD.QTY) > 0) SD
            ON SN.CF_SN = SD.CF_SN
   WHERE NVL (CF.CANCEL_FLAG, 'F') = 'F' AND NVL (CF.APPV_FLAG, 'P') = 'A'
ORDER BY CF.PROD_ID, CF.REVISION, SN.CF_NO