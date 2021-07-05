INSERT INTO KPDBA.COLOR_FOLDER_SN (CF_SN,
                                   CF_SEQ,
                                   CF_STATUS,
                                   CR_DATE,
                                   CR_ORG_ID,
                                   CR_USER_ID)
     VALUES (:AS_CF_SN,
             :AS_CF_SEQ,
             'PEND',
             SYSDATE,
             TO_CHAR (:AS_CR_ORG_ID),
             TO_CHAR (:AS_CR_USER_ID))