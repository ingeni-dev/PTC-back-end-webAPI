WITH ALL_COURSE_VIDEO
     AS (SELECT CT.COURSE_ID,
                CT.COURSE_REVISION,
                CT.TOPIC_ID,
                CT.CANCEL_FLAG,
                CDM.COURSE_DOC_ID,
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
           FROM KPDBA.COURSE_TOPIC CT,
                KPDBA.TOPIC_MASTER CTM,
                KPDBA.TOPIC_MASTER TTM,
                KPDBA.TOPIC_DOCUMENT TD,
                KPDBA.COURSE_DOCUMENT_MASTER CDM
          WHERE     CTM.TOPIC_ID = CT.TOPIC_ID
                AND CTM.TOPIC_TYPE = 'C'
                AND NVL (CT.CANCEL_FLAG, 'F') = 'F'
                AND CTM.TOPIC_ID = TTM.PARENT_TOPIC_ID(+)
                AND TTM.TOPIC_ID = TD.TOPIC_ID(+)
                AND NVL (TD.CANCEL_FLAG, 'F') = 'F'
                AND CDM.COURSE_DOC_ID(+) = TD.COURSE_DOC_ID
                AND NVL (CDM.CANCEL_FLAG, 'F') = 'F')
  SELECT QUERY_ID,
         COURSE_ID,
         COURSE_REVISION,
         COURSE_DESC,
         BEGIN_DATE,
         END_DATE,
         SUM (VIDEO_LENGTH) FULL_TIME_COURSE,
         ROUND (SUM (PERCENT_VIEW_VIDEO) / COUNT (1), 2) PERCENT_VIEW_COURSE
    FROM (SELECT APP_USER_ID,
                 CQ.QUERY_ID,
                 CM.COURSE_ID,
                 ACV.COURSE_REVISION,
                 CM.COURSE_DESC,
                 ACV.DOC_TYPE,
                 C_TOPIC_ID,
                 C_TOPIC_ORDER,
                 C_TOPIC_NAME,
                 T_TOPIC_ID,
                 T_TOPIC_ORDER,
                 T_TOPIC_NAME,
                 DOC_NAME,
                 COURSE_DOC_ID,
                 DOC_PATH,
                 VIDEO_LENGTH,
                 VIDEO_COVER,
                 BEGIN_DATE,
                 END_DATE,
                 DECODE (
                    ACV.DOC_TYPE,
                    'V', KPDBA.DF_CALC_VIEW_VIDEO (CQ.QUERY_ID,
                                                   APP_USER_ID,
                                                   ACV.COURSE_DOC_ID),
                    NULL)
                    PERCENT_VIEW_VIDEO
            FROM KPDBA.COURSE_QUERY CQ,
                 KPDBA.COURSE_MASTER CM,
                 KPDBA.COURSE_APPLICANT CA,
                 ALL_COURSE_VIDEO ACV
           WHERE     QUERY_TYPE = 'O'
                 AND CQ.CANCEL_FLAG <> 'T'
                 AND CQ.COURSE_ID = CM.COURSE_ID
                 AND CA.QUERY_ID = CQ.QUERY_ID
                 AND CQ.COURSE_ID = ACV.COURSE_ID(+)
                 AND APP_USER_ID = :as_emp_id) COOURSE
GROUP BY QUERY_ID,
         COURSE_ID,
         COURSE_REVISION,
         COURSE_DESC,
         BEGIN_DATE,
         END_DATE