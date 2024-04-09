module.exports={
    plugins: [
      ["@semantic-release/commit-analyzer", {
          releaseRules: [
              {"type": "major"  , "release": "major"},
              {"type": "release", "release": "major"},
          ],
          parserOpts: {
              "noteKeywords": ["BREAKING CHANGE", "BREAKING CHANGES", "BREAKING"]
          }
      }],
  
      ["@semantic-release/exec",{
          prepareCmd: `
              set -e
              CR=unicominternal.azurecr.io
              VER=\${nextRelease.version}
              ##vso[build.updatebuildnumber]\${nextRelease.version}
              docker login $CR -u $DOCKER_HUB_USER -p $DOCKER_HUB_PASSWORD
              docker build -f ci/Dockerfile.Service -t $CR/unicom.monitor.service:$VER $STAGING_PATH
              docker push                              $CR/unicom.monitor.service:$VER
          `,
          successCmd: `
              set -e

              TYPE=\${nextRelease.type}
              echo release type is $TYPE
              ##vso[task.setvariable variable=newVer;]yes
              ##vso[build.addbuildtag]release
              ##vso[build.addbuildtag]\${nextRelease.type}
              ##vso[build.addbuildtag]\${nextRelease.version}
              echo -n '##';echo "vso[build.addbuildtag]$BUILD_SOURCEBRANCHNAME"
          `,
      }],
      "@semantic-release/git"
    ],
  
    branches: [
      'master'
    ],
  }