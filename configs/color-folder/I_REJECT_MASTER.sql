INSERT INTO KPDBA.CF_REJECT_MASTER (CF_REJ_REASON,
                                    CF_REJ_REASON_DESC,
                                    CR_DATE,
                                    CR_ORG_ID,
                                    CR_USER_ID)
     VALUES (:AS_CF_REJ_REASON,
             :AS_CF_REJ_REASON_DESC,
             SYSDATE,
             TO_CHAR(:AS_CR_ORG_ID),
             TO_CHAR(:AS_CR_USER_ID))