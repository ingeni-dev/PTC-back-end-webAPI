INSERT INTO KPDBA.COURSE_QUERY_DOCUMENT_DETAIL (QUERY_DOC_ID,
                                                SEQ,
                                                START_TIME,
                                                END_TIME)
     VALUES (:AS_QUERY_DOC_ID,
             TO_NUMBER (:AS_SEQ),
             TO_NUMBER (:AS_START_TIME),
             TO_NUMBER (:AS_END_TIME))