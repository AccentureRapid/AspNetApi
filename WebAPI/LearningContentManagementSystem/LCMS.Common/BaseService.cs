using Accenture.Security.Eso.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Web;

namespace LCMS.Common
{
    public class BaseService
    {
        IEnterprisePrincipal _principal;

        protected IEnterprisePrincipal Principal
        {
            get
            {
                if (_principal == null)
                {
                    _principal = (IEnterprisePrincipal)Thread.CurrentPrincipal;

                    if (_principal == null || string.IsNullOrEmpty(_principal.EnterpriseIdentity.EnterpriseId))
                    {
                        _principal = new DummyEnterprisePrincipal();
                    }
                }

                return _principal;
            }
        }

        protected IEnterpriseIdentity Identity
        {
            get
            {
                return Principal.EnterpriseIdentity;
            }
        }

        protected string EnterpriseId
        {
            get
            {
                return Identity.EnterpriseId;
            }
        }

        protected int PeopleKey
        {
            get
            {
                return Int32.Parse(Identity.PeopleKey);
            }
        }

        public class DummyEnterprisePrincipal : IEnterprisePrincipal
        {
            public IEnterpriseIdentity EnterpriseIdentity
            {
                get
                {
                    return new DummyEnterpriseIdentity();
                }
            }

            public IIdentity Identity
            {
                get
                {
                    return new DummyEnterpriseIdentity();
                }
            }

            public bool IsInRole(string role)
            {
                return true;
            }
        }

        public class DummyEnterpriseIdentity : IEnterpriseIdentity
        {
            public Dictionary<string, string> AllClaims
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string AuthenticationType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string CompanyCode
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string CompanyDescription
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string CostCenterCode
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string CostCenterDescription
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string CountryCode
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string CountryDescription
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string DisplayName
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string EmailAddress
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string EnterpriseId
            {
                get
                {
                    return "mmc_testid_001";
                }
            }

            public string FirstName
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string GeographicUnit
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string GeographicUnitCode
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string InstantMessengerAddress
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string JobFamilyCode
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string JobFamilyDescription
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string LastName
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string Location
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string LocationCode
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string LogOffAddress
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string MiddleInitial
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string Name
            {
                get
                {
                    return "mmc_testid_001";
                }
            }

            public string NonAccentureNumber
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IIdentity OriginalIdentity
            {
                get
                {
                    return this;
                }
            }

            public string PeopleKey
            {
                get
                {
                    return "10000001";
                }
            }

            public string PersonnelNumber
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string SapUserId
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string TelephoneNumber
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string UserPrincipalName
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public EnterpriseUserType UserType
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string WorkforceCode
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string WorkforceDescription
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public string[] GetGroups()
            {
                throw new NotImplementedException();
            }
        }

    }
}
