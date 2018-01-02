
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using Contensive.BaseClasses;

namespace Contensive.Addons.aoToolPanel.Models.Db
{
    public class personModel : baseModel
    {
        //
        //====================================================================================================
        //-- const
        public const string contentName = "people";
        public const string contentTableName = "ccmembers";
        //
        //====================================================================================================
        //public string Address { get; set; }
        //public string Address2 { get; set; }
        //public bool Admin { get; set; }
        //public int AdminMenuModeID { get; set; }
        //public bool AllowBulkEmail { get; set; }
        //public bool AllowToolsPanel { get; set; }
        //public bool AutoLogin { get; set; }
        //public string BillAddress { get; set; }
        //public string BillAddress2 { get; set; }
        //public string BillCity { get; set; }
        //public string BillCompany { get; set; }
        //public string BillCountry { get; set; }
        //public string BillEmail { get; set; }
        //public string BillFax { get; set; }
        //public string BillName { get; set; }
        //public string BillPhone { get; set; }
        //public string BillState { get; set; }
        //public string BillZip { get; set; }
        //public int BirthdayDay { get; set; }
        //public int BirthdayMonth { get; set; }
        //public int BirthdayYear { get; set; }
        //public string City { get; set; }
        //public string Company { get; set; }
        //public int ContentCategoryID { get; set; }
        //public string Country { get; set; }
        //public bool CreatedByVisit { get; set; }
        //public DateTime DateExpires { get; set; }
        //public bool Developer { get; set; }
        public string Email { get; set; }
        //public bool ExcludeFromAnalytics { get; set; }
        //public string Fax { get; set; }
        public string FirstName { get; set; }
        //public string ImageFilename { get; set; }
        //public int LanguageID { get; set; }
        public string LastName { get; set; }
        //public DateTime LastVisit { get; set; }
        //public string nickName { get; set; }
        //public string NotesFilename { get; set; }
        //public int OrganizationID { get; set; }
        public string Password { get; set; }
        //public string Phone { get; set; }
        //public string ResumeFilename { get; set; }
        //public string ShipAddress { get; set; }
        //public string ShipAddress2 { get; set; }
        //public string ShipCity { get; set; }
        //public string ShipCompany { get; set; }
        //public string ShipCountry { get; set; }
        //public string ShipName { get; set; }
        //public string ShipPhone { get; set; }
        //public string ShipState { get; set; }
        //public string ShipZip { get; set; }
        //public string State { get; set; }
        //public string StyleFilename { get; set; }
        //public string ThumbnailFilename { get; set; }
        //public string Title { get; set; }
        public string Username { get; set; }
        //public int Visits { get; set; }
        //public string Zip { get; set; }
        //
        //====================================================================================================
        public static personModel @add(CPBaseClass cp)
        {
            return @add<personModel>(cp);
        }
        //
        //====================================================================================================
        public static personModel create(CPBaseClass cp, int recordId)
        {
            return create<personModel>(cp, recordId);
        }
        //
        //====================================================================================================
        public static personModel create(CPBaseClass cp, string recordGuid)
        {
            return create<personModel>(cp, recordGuid);
        }
        //
        //====================================================================================================
        public static personModel createByName(CPBaseClass cp, string recordName)
        {
            return createByName<personModel>(cp, recordName);
        }
        //
        //====================================================================================================
        public void save(CPBaseClass cp)
        {
            base.save(cp);
        }
        //
        //====================================================================================================
        public static void delete(CPBaseClass cp, int recordId)
        {
            delete<personModel>(cp, recordId);
        }
        //
        //====================================================================================================
        public static void delete(CPBaseClass cp, string ccGuid)
        {
            delete<personModel>(cp, ccGuid);
        }
        //
        //====================================================================================================
        public static List<personModel> createList(CPBaseClass cp, string sqlCriteria, string sqlOrderBy = "id")
        {
            return createList<personModel>(cp, sqlCriteria, sqlOrderBy);
        }
        //
        //====================================================================================================
        public static string getRecordName(CPBaseClass cp, int recordId)
        {
            return baseModel.getRecordName<personModel>(cp, recordId);
        }
        //
        //====================================================================================================
        public static string getRecordName(CPBaseClass cp, string ccGuid)
        {
            return baseModel.getRecordName<personModel>(cp, ccGuid);
        }
        //
        //====================================================================================================
        public static int getRecordId(CPBaseClass cp, string ccGuid)
        {
            return baseModel.getRecordId<personModel>(cp, ccGuid);
        }

        //
        //====================================================================================================
        //
        public personModel Clone(CPBaseClass cp)
        {
            personModel result = (personModel)this.Clone();
            result.id = cp.Content.AddRecord(contentName);
            result.ccguid = cp.Utils.CreateGuid();
            result.save(cp);
            return result;
        }
        //
        //====================================================================================================
        //
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
