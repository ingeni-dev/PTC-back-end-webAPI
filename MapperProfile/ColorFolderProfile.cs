using PTC_back_end_webAPI.Models.ColorFolder;

namespace PTC_back_end_webAPI.MapperProfile
{
    public class ColorFolderProfile : AutoMapper.Profile
    {
        public ColorFolderProfile()
        {
            CreateMap<Product, ProductMap>()
                .ForMember(d => d.prodID, o => o.MapFrom(s => s.PROD_ID))
                .ForMember(d => d.revision, o => o.MapFrom(s => s.REVISION))
                .ForMember(d => d.prodName, o => o.MapFrom(s => s.PROD_DESC));

            CreateMap<AllowFolder, AllowFolderMap>()
                .ForMember(d => d.cfSEQ, o => o.MapFrom(s => s.CF_SEQ))
                .ForMember(d => d.prodID, o => o.MapFrom(s => s.PROD_ID))
                .ForMember(d => d.prodName, o => o.MapFrom(s => s.PROD_DESC))
                .ForMember(d => d.saleName, o => o.MapFrom(s => s.SALE_NAME))
                .ForMember(d => d.withdDate, o => o.MapFrom(s => s.WITHD_DATE))
                .ForMember(d => d.crQTY, o => o.MapFrom(s => s.CR_QTY))
                .ForMember(d => d.days, o => o.MapFrom(s => s.DAYS))
                .ForMember(d => d.collectionDate, o => o.MapFrom(s => s.COLLECTION_DATE))
                .ForMember(d => d.cfType, o => o.MapFrom(s => s.CF_TYPE))
                .ForMember(d => d.state, o => o.MapFrom(s => s.STATE));

            CreateMap<UserSale, UserSaleMap>()
                .ForMember(d => d.saleID, o => o.MapFrom(s => s.SALE_ID))
                .ForMember(d => d.saleName, o => o.MapFrom(s => s.SALE_NAME));

            CreateMap<RejectMessage, RejectMessageMap>()
               .ForMember(d => d.rejID, o => o.MapFrom(s => s.CF_REJ_REASON))
               .ForMember(d => d.rejMessage, o => o.MapFrom(s => s.CF_REJ_REASON_DESC));

            CreateMap<ColorFolderDetail, ColorFolderDetailMap>()
               .ForMember(d => d.cfSN, o => o.MapFrom(s => s.CF_SN))
               .ForMember(d => d.locDetail, o => o.MapFrom(s => s.LOC_DETAIL))
               .ForMember(d => d.prodID, o => o.MapFrom(s => s.PROD_ID))
               .ForMember(d => d.prodDESC, o => o.MapFrom(s => s.PROD_DESC))
               .ForMember(d => d.snNO, o => o.MapFrom(s => s.SN_NO))
               .ForMember(d => d.tranDate, o => o.MapFrom(s => s.TRAN_DATE))
               .ForMember(d => d.cfStatusDESC, o => o.MapFrom(s => s.CF_STATUS_DESC));

            CreateMap<Files, FilesMap>()
                .ForMember(d => d.prodID, o => o.MapFrom(s => s.PROD_ID))
                .ForMember(d => d.prodDesc, o => o.MapFrom(s => s.PROD_DESC))
                .ForMember(d => d.qrCode, o => o.MapFrom(s => s.QR_CODE))
                .ForMember(d => d.num, o => o.MapFrom(s => s.NUM));

            CreateMap<Folder, FolderMap>()
               .ForMember(d => d.cfSn, o => o.MapFrom(s => s.CF_SN))
               .ForMember(d => d.prodDesc, o => o.MapFrom(s => s.PROD_DESC))
               .ForMember(d => d.appDate, o => o.MapFrom(s => s.APP_DATE))
               .ForMember(d => d.custName, o => o.MapFrom(s => s.CUST_NAME))
               .ForMember(d => d.expireDate, o => o.MapFrom(s => s.EXPIRE_DATE))
               .ForMember(d => d.runningNo, o => o.MapFrom(s => s.RUNNING_NO))
               .ForMember(d => d.approval, o => o.MapFrom(s => s.APPROVAL))
               .ForMember(d => d.qrCode, o => o.MapFrom(s => s.QR_CODE))
               .ForMember(d => d.num, o => o.MapFrom(s => s.NUM));

            CreateMap<Job, JobMap>()
              .ForMember(d => d.jobID, o => o.MapFrom(s => s.JOB_ID))
              .ForMember(d => d.custName, o => o.MapFrom(s => s.CUST_NAME))
              .ForMember(d => d.custID, o => o.MapFrom(s => s.CUST_ID));

            CreateMap<CustDetail, CustDetailMap>()
              .ForMember(d => d.ord, o => o.MapFrom(s => s.ORD))
              .ForMember(d => d.custName, o => o.MapFrom(s => s.CUST_NAME))
              .ForMember(d => d.custID, o => o.MapFrom(s => s.CUST_ID));

            CreateMap<KeepScanLoc, KeepScanLocMap>()
                .ForMember(d => d.locID, o => o.MapFrom(s => s.LOC_ID))
                .ForMember(d => d.locDetail, o => o.MapFrom(s => s.LOC_DETAIL))
                .ForMember(d => d.warehouseID, o => o.MapFrom(s => s.WAREHOUSE_ID))
                .ForMember(d => d.compID, o => o.MapFrom(s => s.COMP_ID));

            CreateMap<KeepScanSN, KeepScanSNMap>()
                .ForMember(d => d.cfSN, o => o.MapFrom(s => s.CF_SN))
                .ForMember(d => d.cfSEQ, o => o.MapFrom(s => s.CF_SEQ))
                .ForMember(d => d.cfStatus, o => o.MapFrom(s => s.CF_STATUS))
                .ForMember(d => d.prodDESC, o => o.MapFrom(s => s.PROD_DESC))
                .ForMember(d => d.availableFlag, o => o.MapFrom(s => s.AVAILABLE_FLAG))
                .ForMember(d => d.snNO, o => o.MapFrom(s => s.SN_NO));
        }

    }
}