WITH COURSE_LECT
     AS (SELECT APP_USER_ID,
                CQ.QUERY_ID,
                CM.COURSE_DESC,
                BEGIN_DATE,
                END_DATE,
                TD.COURSE_DOC_ID,
                CT.COURSE_ID,
                CT.COURSE_REVISION,
                CT.TOPIC_ID,
                CT.CANCEL_FLAG,
                CTM.TOPIC_ID C_TOPIC_ID,
                CTM.TOPIC_ORDER C_TOPIC_ORDER,
                CTM.TOPIC_NAME C_TOPIC_NAME,
                CTM.PARENT_TOPIC_ID C_PARENT_TOPIC_ID,
                TTM.TOPIC_ID T_TOPIC_ID,
                TTM.TOPIC_ORDER T_TOPIC_ORDER,
                TTM.TOPIC_NAME T_TOPIC_NAME,
                TTM.PARENT_TOPIC_ID T_PARENT_TOPIC_ID,
                CDM.DOC_TYPE,
                CDM.DOC_NAME,
                CDM.DOC_PATH,
                CDM.VIDEO_COVER,
                CDM.VIDEO_LENGTH
           FROM KPDBA.COURSE_QUERY CQ,
                KPDBA.COURSE_MASTER CM,
                KPDBA.COURSE_APPLICANT CA,
                KPDBA.COURSE_TOPIC CT,
                KPDBA.TOPIC_MASTER CTM,
                KPDBA.TOPIC_MASTER TTM,
                KPDBA.TOPIC_DOCUMENT TD,
                KPDBA.COURSE_DOCUMENT_MASTER CDM
          WHERE     QUERY_TYPE = 'O'
                AND CQ.CANCEL_FLAG <> 'T'
                AND CQ.COURSE_ID = CM.COURSE_ID
                AND CA.QUERY_ID = CQ.QUERY_ID
                AND CT.COURSE_ID = CM.COURSE_ID
                AND CT.COURSE_REVISION = 0
                AND NVL (CT.CANCEL_FLAG, 'F') = 'F'
                AND CTM.TOPIC_ID = CT.TOPIC_ID
                AND CTM.TOPIC_TYPE = 'C'
                AND CTM.TOPIC_ID = TTM.PARENT_TOPIC_ID(+)
                AND TTM.TOPIC_ID = TD.TOPIC_ID(+)
                AND NVL (TD.CANCEL_FLAG, 'F') = 'F'
                AND CDM.COURSE_DOC_ID(+) = TD.COURSE_DOC_ID
                AND NVL (CDM.CANCEL_FLAG, 'F') = 'F'
                AND APP_USER_ID LIKE :as_emp_id
         UNION ALL
         SELECT CA.APP_EMP_ID,
                CQ.QUERY_ID,
                CM.DOC_NAME COURSE_DESC,
                COURSE_DATE BEGIN_DATE,
                (COURSE_DATE + NUM_OF_HOUR / 24 + NUM_OF_MIN / 24 / 60)
                   END_DATE,
                TD.COURSE_DOC_ID,
                CT.COURSE_ID,
                CT.COURSE_REVISION,
                CT.TOPIC_ID,
                CT.CANCEL_FLAG,
                CTM.TOPIC_ID C_TOPIC_ID,
                CTM.TOPIC_ORDER C_TOPIC_ORDER,
                CTM.TOPIC_NAME C_TOPIC_NAME,
                CTM.PARENT_TOPIC_ID C_PARENT_TOPIC_ID,
                TTM.TOPIC_ID T_TOPIC_ID,
                TTM.TOPIC_ORDER T_TOPIC_ORDER,
                TTM.TOPIC_NAME T_TOPIC_NAME,
                TTM.PARENT_TOPIC_ID T_PARENT_TOPIC_ID,
                CDM.DOC_TYPE,
                CDM.DOC_NAME,
                CDM.DOC_PATH,
                CDM.VIDEO_COVER,
                CDM.VIDEO_LENGTH
           FROM KPDBA.ISO_COURSE_QUERY CQ,
                KPDBA.ISO_MASTER CM,
                KPDBA.ISO_COURSE_APPLICANT CA,
                KPDBA.COURSE_TOPIC CT,
                KPDBA.TOPIC_MASTER CTM,
                KPDBA.TOPIC_MASTER TTM,
                KPDBA.TOPIC_DOCUMENT TD,
                KPDBA.COURSE_DOCUMENT_MASTER CDM
          WHERE     QUERY_TYPE = 'O'
                AND CQ.CANCEL_FLAG <> 'T'
                AND CQ.DOC_CODE = CM.DOC_CODE
                AND CQ.DOC_REVISION = CM.DOC_REVISION
                AND CA.QUERY_ID = CQ.QUERY_ID
                AND CT.COURSE_ID = CM.DOC_CODE
                AND CT.COURSE_REVISION = CM.DOC_REVISION
                AND NVL (CT.CANCEL_FLAG, 'F') = 'F'
                AND CTM.TOPIC_ID = CT.TOPIC_ID
                AND CTM.TOPIC_TYPE = 'C'
                AND CTM.TOPIC_ID = TTM.PARENT_TOPIC_ID(+)
                AND TTM.TOPIC_ID = TD.TOPIC_ID(+)
                AND NVL (TD.CANCEL_FLAG, 'F') = 'F'
                AND CDM.COURSE_DOC_ID(+) = TD.COURSE_DOC_ID
                AND NVL (CDM.CANCEL_FLAG, 'F') = 'F'
                AND CA.APP_EMP_ID LIKE :as_emp_id)
  SELECT CL.*,
         CQD.COUNT,
         CQD.CURR_TIME,
         CQD.LAST_VISIT
    FROM COURSE_LECT CL, KPDBA.COURSE_QUERY_DOCUMENT CQD
   WHERE CL.COURSE_DOC_ID = CQD.COURSE_DOC_ID AND CQD.QUERY_ID = CL.QUERY_ID
ORDER BY LAST_VISIT DESC