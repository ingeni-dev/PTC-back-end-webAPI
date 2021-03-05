INSERT INTO KPDBA.TOPIC_DOCUMENT (TOPIC_ID,
                                  COURSE_DOC_ID,
                                  DOC_ORDER,
                                  CR_DATE,
                                  CR_ORG_ID,
                                  CR_USER_ID,
                                  CANCEL_FLAG)
     VALUES (:AD_TOPIC_ID,
             :AD_COURSE_DOC_ID,
             TO_NUMBER (:AD_DOC_ORDER),
             SYSDATE,
             TO_CHAR (:AD_ORG_ID),
             TO_CHAR (:AD_USER_ID),
             :AD_CANCEL_FLAG)