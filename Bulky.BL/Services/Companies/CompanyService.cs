using AutoMapper;
using Bulky.BL.Models.Companies;
using Bulky.DataAccess.Entities;
using Bulky.DataAccess.Exceptions;
using Bulky.DataAccess.Repository.UnitOfWork.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Services.Companies
{
    public class CompanyService(IUnitOfWork _unitOfWork ,IMapper _mapper) : ICompanyService
    {
        public async Task<int> CreateCompanyAsync(CompanyDto company)
        {
            if (company == null)
                throw new BadRequestException(["Company data is required."]);

            var companydb = _mapper.Map<Company>(company);

            await _unitOfWork.CompanyRepository.AddAsync(companydb);

            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> DeleteCompanyAsync(int? id)
        {
            if (id == null)
                throw new BadRequestException(["id is required."]);

            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id.Value);

            if (company == null)
                throw new CompanyNotFoundException(id.Value);

            _unitOfWork.CompanyRepository.Delete(company);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            var companies = await _unitOfWork.CompanyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<CompanyDto?> GetCompanyByIdAsync(int? id)
        {
            if (id == null)
                throw new BadRequestException(["Id is required."]);

            var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id.Value);

            if (company == null)
                throw new CompanyNotFoundException(id.Value);

            return _mapper.Map<CompanyDto>(company);
            

        }

        public async Task<int> UpdateCompanyAsync(CompanyDto company)
        {
            if (company == null)
                throw new BadRequestException(["Company data is required."]);

            var companyFromDB = await _unitOfWork.CompanyRepository.GetByIdAsync(company.Id);

            if (companyFromDB == null)
                throw new CompanyNotFoundException(company.Id);

            _mapper.Map(company, companyFromDB);

            _unitOfWork.CompanyRepository.Update(companyFromDB);

            return await _unitOfWork.SaveChangesAsync();
        }
    }
}
