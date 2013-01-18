
require 'httparty'
require 'json'
require 'openssl'
require 'base64'

module Webtrends
  module Client
    module Authentication
      class TokenRequest
        include HTTParty
        format :json
        
        def initialize(client_id, client_secret)
          @audience = "auth.webtrends.com"
          @scope = "sapi.webtrends.com"
          @auth_url = "https://sauth.webtrends.com/v1/token"
          @expiration = (Time.new + 30*60).to_i
          
          @client_id = client_id
          @client_secret = client_secret
        end

        def execute
          assertion = jwt_assertion
          jwt = {
            client_id: @client_id,
            client_assertion: assertion
          }
          options = {
            :body => jwt
          }
          result = self.class.post(@auth_url, options)
          raise "Bad request" unless !result['access_token'].nil?
          result['access_token']
        end

        private
                
        def jwt_assertion
          payload = "#{jwt_header}.#{jwt_claims}"
          hmac = OpenSSL::HMAC.digest(OpenSSL::Digest::Digest.new("sha256"), @client_secret, payload)
          signature = Base64.encode64(hmac)
          "#{payload}.#{signature}"
        end
        
        def jwt_header
          json = { typ: "JWT", alg: "HS256" }.to_json
          Base64.urlsafe_encode64(json)
        end

        def jwt_claims
          json = {
            iss: @client_id,
            prn: @client_id,
            aud: @audience,
            exp: @expiration,
            scope: @scope
          }.to_json
          Base64.encode64(json)
        end
        
      end
    end
  end
end
