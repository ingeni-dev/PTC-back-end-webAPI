INSERT INTO KPDBA.CF_STOCK_DETAIL (TRAN_ID,
                                   TRAN_SEQ,
                                   TRAN_TYPE,
                                   TRAN_DATE,
                                   CF_SN,
                                   QTY,
                                   COMP_ID,
                                   WAREHOUSE_ID,
                                   LOC_ID,
                                   EMP_ID,
                                   STATUS,
                                   REMARK,
                                   CR_DATE,
                                   CR_ORG_ID,
                                   CR_USER_ID)
     VALUES (:AS_TRAN_ID,
             TO_NUMBER (:AS_TRAN_SEQ),
             TO_NUMBER (:AS_TRAN_TYPE),
             TO_DATE (:AS_TRAN_DATE, 'mm/dd/yyyy hh:mi:ss am'),
             :AS_CF_SN,
             TO_NUMBER (:AS_QTY),
             :AS_COMP_ID,
             :AS_WAREHOUSE_ID,
             :AS_LOC_ID,
             TO_CHAR (:AS_EMP_ID),
             TO_CHAR (:AS_STATUS),
             :AS_REMARK,
              TO_DATE (:AS_CR_DATE, 'mm/dd/yyyy hh:mi:ss am'),
             TO_CHAR (:AS_CR_ORG_ID),
             TO_CHAR (:AS_CR_USER_ID))