SELECT SUBSTR (PROD_ID, 7) || '-' || REVISION PROD_ID, PROD_DESC
  FROM (  SELECT DISTINCT
                 P.PROD_ID,
                 P.REVISION,
                 P.PROD_DESC,
                 CASE
                    WHEN    INSTR (P.PROD_ID || '-' || P.REVISION,
                                   :AS_TXT_SEARCH) > 0
                         OR INSTR (P.PROD_ID || '/' || P.REVISION,
                                   :AS_TXT_SEARCH) > 0
                    THEN
                       1
                    WHEN INSTR (P.PROD_ID, :AS_TXT_SEARCH) > 0
                    THEN
                       2
                    WHEN INSTR (P.PROD_DESC, :AS_TXT_SEARCH) > 0
                    THEN
                       3
                    WHEN INSTR (J.JOB_ID, :AS_TXT_SEARCH) > 0
                    THEN
                       4
                    WHEN INSTR (PC.CUST_ID, :AS_TXT_SEARCH) > 0
                    THEN
                       5
                    WHEN INSTR (PC.CUST_NAME, :AS_TXT_SEARCH) > 0
                    THEN
                       6
                    ELSE
                       7
                 END
                    ANS
            FROM KPDBA.PRODUCT P
                 LEFT JOIN (SELECT JD.JOB_ID, JD.PROD_ID, JD.REVISION
                              FROM    KPDBA.JOB J
                                   JOIN
                                      KPDBA.JOB_DETAIL JD
                                   ON J.JOB_ID = JD.JOB_ID
                             WHERE J.STATUS = 'O') J
                    ON P.PROD_ID = J.PROD_ID AND P.REVISION = J.REVISION
                 LEFT JOIN (SELECT C.CUST_ID,
                                   C.CUST_NAME,
                                   PC.PROD_ID,
                                   PC.REVISION
                              FROM    KPDBA.PROD_CUST PC
                                   JOIN
                                      KPDBA.CUSTOMER C
                                   ON C.CUST_ID = PC.CUST_ID) PC
                    ON P.PROD_ID = PC.PROD_ID AND P.REVISION = PC.REVISION
           WHERE     P.CANCEL_FLAG = 'F'
                 AND (   INSTR (P.PROD_ID || '-' || P.REVISION, :AS_TXT_SEARCH) >
                            0
                      OR INSTR (P.PROD_ID || '/' || P.REVISION, :AS_TXT_SEARCH) >
                            0
                      OR INSTR (P.PROD_ID, :AS_TXT_SEARCH) > 0
                      OR INSTR (P.REVISION, :AS_TXT_SEARCH) > 0
                      OR INSTR (P.PROD_DESC, :AS_TXT_SEARCH) > 0
                      OR INSTR (J.JOB_ID, :AS_TXT_SEARCH) > 0
                      OR INSTR (PC.CUST_ID, :AS_TXT_SEARCH) > 0
                      OR INSTR (PC.CUST_NAME, :AS_TXT_SEARCH) > 0)
        ORDER BY 4, P.PROD_ID, P.REVISION)
 WHERE ROWNUM <= 25