using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.CustomModels
{
    public class Enumeration
    {
        public enum PushNotificationType
        {
            Announcement = 1,
            OrderAccepted = 2,
            OrderRejected = 3,
            OrderAssignedToDeliverer = 4,
            OrderDispatched = 5,
            OrderCompleted = 6
        }

        public enum NotificationTargetAudienceTypes
        {
            UserAndDeliverer = 1,
            User = 2,
            Deliverer = 3,
            IndividualUser

        }

        public enum BoxCategoryOptions
        {
            Junior = 1,
            Monthly = 2,
            ProBox = 3,
            HallOfFame = 4
        }

        public enum PlatformTypes
        {
            Android = 1,
            Ios = 2,
            Web = 3
        }

        public enum WeightUnits
        {
            gm = 1,
            kg = 2
        }

        public enum StatusCode
        {
            NotVerified = 1,
            Verified = 2
        }

        public enum NotificationStatus
        {
            Unread,
            Read
        }

        public enum DelivererTypes
        {
            Salaried,
            Freelance
        }

        public enum PaymentMethods
        {
            CashOnDelivery,
            CreditCard,
            DebitCard
        }

        public enum PaymentCardTypes
        {
            CreditCard = 1,
            DebitCard = 2
        }

        public enum OrderStatuses
        {
            Initiated,
            Accepted,
            Rejected,
            InProgress,
            ReadyForDelivery,
            AssignedToDeliverer, //equivalent to deliverer initiated 3
            DelivererInProgress,
            Dispatched,
            Completed
        }

        public enum UserAddressTypes
        {
            Residential,
            Business,
            Postal,
            POBox,
            MailTo,
            DeliveryTo
        }

        public enum ApnsEnvironmentTypes
        {
            Sandbox,
            Production
        }

        public enum ApplicationTypes
        {
            PlayStore,
            Enterprise
        }

        public enum CartItemTypes
        {
            Product,
            Package,
            Offer_Product,
            Offer_Package,
            Box
        }

        public enum PostVisibilityTypes
        {
            Public = 1,
            Follower = 2,
            OnlyMe = 3
        }

        public enum RiskLevelTypes
        {
            High = 1,
            Medium = 2,
            Low = 3
        }

        public enum MediaTypes
        {
            Image = 1,
            Video = 2
        }

        public enum PostTaggingPrivacyTypes
        {
            Anyone = 1,
            Following = 2,
            Follower = 3,
            NotAllowed = 4
        }

        public enum DirectMessagePrivacyTypes
        {
            Anyone = 1,
            Following = 2,
            Follower = 3,
            NotAllowed = 4,
            FollowersAndFollowing = 5
        }

        public enum ReportPostTypes
        {
            Spam = 1,
            HateSpeech = 2,
            Violence = 3,
            Duplicate = 4
        }

        public enum GroupMemberStatusTypes
        {
            YetToJoin = 1,                                      //members who are invited by admin but have not accepted the request yet
            Pending = 2,                                        //members who have sent a request to join the group but the request is not accepted by admin
            Accepted = 3,
            Rejected = 4,
            CancelRequest = 5,
            LeftByUser = 6,
            LeftUserByAdmin = 7,
            DoesNotExist = 8                        //this does not exist in database
        }

        public enum ReportUserStatusTypes
        {
            Spam = 1,
            AbusiveHateful = 2,
        }
        public enum PollTypes
        {
            None = 0,
            Text = 1,
            Media = 2
        }

        public enum PostTypes
        {
            Post = 1,
            Incident = 2
        }
        public enum GenderType
        {
            Female = 0,
            Male = 1,
        }
        public enum EmailTypes
        {
            Welcome,
            PhoneVerification,
            ForgetPassword,
            ResetPassword,
            UpdateProfile,
            AccountSuspensionWarning,
            AccountSuspended,
            PostDeletionWarning,
            PostDeleted,
            NewLogin,
            UpdateCover,
            EmailVerification,
            General
        }
        public enum SubscriptionStatus
        {
            InActive,
            Active
        }

        public enum PaymentStatuses
        {
            Pending = 1,
            Completed = 2
        }
        public enum RiscoEntityTypes
        {
            Product = 1,
            Category = 2,
            Store = 3,
            Package = 4,
            Admin = 5,
            Offer = 6,
            Box = 7,
            Post = 8,
            LikeOnPost = 9,
            CommentOnPost = 10,
            ReplyOnComment = 11,
            Group = 12,
            Follow = 13,
            Follower = 14,
            SharePost = 15,
            SendGroupRequest = 16,
            AcceptGroupRequest = 17,
            ProfileImage = 18,
            NotificationSettings = 19,
            AccountSettings = 20,
            PrivacySettings = 21,
            ChangePassword = 22,
            AddInGroup = 23,
            LikeOnComment = 24,
            BlockUser = 25,
            ReportUser = 26,
            ReportPost = 27,
            CreateGroup = 28,
            FAQ = 29,
            UpdateCover = 30,
            RejectGroupRequest = 31,
            CancelGroupRequest = 32
        }
        public enum PayfortCommands
        {
            AUTHORIZE,
            CAPTURE,
            VOID,
            PURCHASE
        }
        public enum SocialLoginType
        {
            Google = 6,
            Facebook = 7,
            Instagram = 8,
            Twitter = 9
        }

        public enum PaymentStatus
        {
            Pending,
            Paid
        }
        public enum DeviceTypes
        {
            Android = 1,
            IOS = 2,
            Web = 3
        }
        public enum PollOptions
        {
            Questions = 0,
            Images = 1
        }
        public enum FileType
        {
            Photo = 1,
            Video = 2,
            Audio = 3,
            Excel = 4,
            Pdf = 5,
            Doc = 6,
            Pppt = 7,
            Txt = 8,
            Other = 9


        }

        public enum NotificationTypes
        {
            YouDontFollow = 1,
            WhoDontFollowYou = 2,
            WhoHaventConfirmEmail = 3,
            WhoHaventConfirmPhoneNumber = 4,
            Posts = 5,
            Incidents = 6,
            Groups = 7,
            EmailNotification = 8,
            Newposts = 9,
            NewIncidents = 10,
            DirectMessages = 11,
            RiscoMessagesPostToYou = 12,
            NewsAboutUpdatesToRiscoProductsAndServices = 13,
            NewsAboutRiscoProductsAndServices = 14,
            SuggestionsForRecommendedAccount = 15,
            DirectMessagesSMS = 16,
            Likes = 17,
            NewFollowers = 18,
            NewAddingInGroup = 19,
            SharingMessages = 20,
            CrisesAndEmergencyAlerts = 21,
            NewProfileJoinGroup = 22,
            NewMembersRequestToJoin = 23,
            RemoveProfileFromGroup = 24
        }
    }
}
