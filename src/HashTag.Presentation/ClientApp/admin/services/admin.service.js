"use strict";

(function () {

    angular
        .module("admin")
        .service("adminService", adminService);

    adminService.$inject = ["communicationService"];

    function adminService(communicationService) {
        const service = {
            buildClusters: buildClusters,
            savePredictionClasses: savePredictionClasses,
            saveSamplesPhotosPredictions: saveSamplesPhotosPredictions,
            kmeansResearch: kmeansResearch,
            createTestDirectory: createTestDirectory,
            bindPhotosToClusters: bindPhotosToClusters,
            getKMeansResearchData: getKMeansResearchData
        };

        return service;

        function buildClusters() {
            return communicationService.get("api/admin/buildClusters");
        }

        function savePredictionClasses() {
            return communicationService.get("api/admin/savePredictionClasses");
        }

        function saveSamplesPhotosPredictions() {
            return communicationService.get("api/admin/saveSamplesPhotosPredictions");
        }

        function kmeansResearch() {
            return communicationService.get("api/admin/kmeansResearch");
        }

        function createTestDirectory() {
            return communicationService.get("api/admin/createTestDirectory");
        }

        function bindPhotosToClusters() {
            return communicationService.get("api/admin/bindPhotosToClusters");
        }

        function getKMeansResearchData() {
            return communicationService.get("api/admin/getKMeansResearchData");
        }
    }

}());